using SecuredSpace.Battle.Tank;
using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.UnityExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.Components.Battle.Bonus;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.Bonus;
using UTanksClient.Extensions;
using UTanksClient.Network.NetworkEvents.FastGameEvents;
using UTanksClient.Services;
using Assets.ClientCore.CoreImpl.ECS.Components.Battle.Bonus;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.ClientControl.Model;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components;

namespace SecuredSpace.Battle.Drop
{
    public class DropManager : IComponentManager
    {
        public ConfigObj objConfig;
        public GameObject ParachuteVisualObj;
        public GameObject ParachuteObj;
        public GameObject CordExample;
        public GameObject CordsHeader;
        public GameObject Box;
        public GameObject BoundPoint;
        public ItemCard boxResource;
        //public ItemResourcesDB boxResourceDB;
        private BattleManager battleManager = null;
        public BattleManager BattleManager
        {
            get
            {
                if(battleManager == null)
                    this.battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>((this.ConnectPoint as ECSComponent).ownerEntity);
                return this.battleManager;
            }
        }
        public BonusComponent bonusComponent => (this.ConnectPoint as BonusComponent);
        public BonusState dropState;
        private Rigidbody rigidbody;
        private Vector3 cacheScale;
        private ConstantForce constantForce;
        public bool Pendulum = true;
        public float PendulumFactor = 3;
        public float TakenUpperDistance = 3;
        public float DespawnFactor = 1;
        public float nowPendulum = 0f;
        public float pendulumSmooth = 0.001f;
        [Space(10)]
        public bool disablePendulum = false;
        public bool Landed = false;
        public bool Taken = false;
        private bool localTaken = false;
        public bool Despawned = false;
        public bool ParachuteHidden = false;
        [Space(10)]
        public float hiddingScaling = 0.1f;
        public float boxTakenUpScaling = 0.1f;
        [HideInInspector]
        public bool Created = false;
        private void Start()
        {
            rigidbody = this.GetComponent<Rigidbody>();
            constantForce = this.GetComponent<ConstantForce>();
            cacheScale = this.transform.localScale;
        }
        private static void GenerateDrop(BonusComponent dropEntity)
        {
            var DropObjectExample = ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.Drop);
            var bonusComponent = dropEntity;

            var dropGObject = Instantiate(DropObjectExample);
            var dropObject = dropGObject.GetComponent<DropManager>();
            dropObject.ConnectPoint = (dropEntity.componentManagers[typeof(DropManager)] as DropManager).ConnectPoint;
            dropGObject.transform.SetParent(dropObject.BattleManager.MapManager.DropSpace.transform);
            dropObject.InitalizeDrop(bonusComponent);
            dropObject.Created = true;
            dropEntity.componentManagers[typeof(DropManager)] = dropObject;
        }

        public static void UpdateDrop(BonusComponent dropEntity)
        {
            DropManager dropManager = null;
            //var battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(dropEntity.ownerEntity);
            BattleManager.LoadedBattleClientAction(dropEntity.ownerEntity, (nul) =>
            {
                if (dropEntity.componentManagers.TryGetValue<DropManager>(out dropManager) && !dropManager.Created)
                {
                    GenerateDrop(dropEntity);
                    dropEntity.componentManagers.TryGetValue<DropManager>(out dropManager);
                }
                if (dropEntity.bonusState == BonusState.Dropped)
                {

                }
            }, false);
            
            
        }

        public void OnEnable()
        {
            //InitalizeDrop(new BonusComponent() {
            //    ConfigPath = "battle\\bonus\\armor"
            //});
        }

        int Side = 1;
        protected override void FixedUpdateManager()
        {
            base.FixedUpdateManager();
            PendulumProcess();
            LandingProcess();
            DespawnProcess();
            TakenProcess();
        }

        public void PendulumProcess()
        {
            if (!disablePendulum)
            {
                if (Mathf.Abs(nowPendulum) > PendulumFactor)
                {
                    Side *= -1;
                }
                if (Side > 0)
                {
                    nowPendulum += PendulumFactor * Time.fixedDeltaTime * pendulumSmooth;
                }
                else
                {
                    nowPendulum -= PendulumFactor * Time.fixedDeltaTime * pendulumSmooth;
                }
                this.transform.rotation = Quaternion.Euler(new Vector3(nowPendulum, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z));
            }
        }

        public void LandingProcess()
        {
            if (Landed && !ParachuteHidden)
            {
                if (ParachuteObj.transform.localScale.y > 0.1f)
                    ParachuteObj.transform.localScale = new Vector3(1, ParachuteObj.transform.localScale.y - Time.fixedDeltaTime * hiddingScaling, 1);
                if (ParachuteObj.transform.localScale.y / 1f < 0.5f)
                {
                    CordsHeader.SetActive(false);
                }
                if(rigidbody.drag > 0)
                {
                    rigidbody.drag = 0;
                    rigidbody.angularDrag = 0;
                }
                //try
                //{
                //    ParachuteVisualObj.GetComponent<Cloth>().enabled = true;
                //}
                //catch { }
                foreach (var material in ParachuteVisualObj.GetComponent<Renderer>().materials)
                {
                    var color = material.GetColor("_ColorShade1");
                    color.a = ParachuteObj.transform.localScale.y / 1f;
                    material.SetColor("_ColorShade1", color);
                }
                if (ParachuteObj.transform.localScale.y / 1f < 0.1f)
                {
                    ParachuteHidden = true;
                    ParachuteObj.SetActive(false);
                }
            }
        }

        public void DespawnProcess()
        {
            if (Despawned)
            {
                rigidbody.isKinematic = true;
                if (Box.transform.localScale.y > 0f)
                    Box.transform.localScale = new Vector3(Box.transform.localScale.x - Time.fixedDeltaTime * DespawnFactor, Box.transform.localScale.y - Time.fixedDeltaTime * DespawnFactor, Box.transform.localScale.z - Time.fixedDeltaTime * DespawnFactor);
                var boxMaterial = Box.GetComponent<MeshRenderer>().material;
                var color = boxMaterial.GetColor("_ColorShade1");
                color.a = Box.transform.localScale.y / cacheScale.y;
                boxMaterial.SetColor("_ColorShade1", color);
                if (Box.transform.localScale.y < 0f)
                {
                    Box.SetActive(false);
                    this.gameObject.SetActive(false);
                    Destroy(this.gameObject);
                }
            }
        }

        public void TakenProcess()
        {
            if (Taken)
            {
                rigidbody.isKinematic = true;
                if (Box.transform.localPosition.y < TakenUpperDistance)
                    Box.transform.localPosition = new Vector3(0, Box.transform.localPosition.y + Time.fixedDeltaTime * boxTakenUpScaling, 0);
                var boxMaterial = Box.GetComponent<MeshRenderer>().material;
                var color = boxMaterial.GetColor("_ColorShade1");
                color.a = 1f - Box.transform.localPosition.y / TakenUpperDistance;
                boxMaterial.SetColor("_ColorShade1", color);
                if (Box.transform.localPosition.y >= TakenUpperDistance)
                {
                    Box.SetActive(false);
                    this.gameObject.SetActive(false);
                    Destroy(this.gameObject);
                }
            }
        }

        //int Side = 1;
        //bool returing = false;
        //public void FixedUpdate()
        //{
        //    var pendulumDelta = PendulumFactor * Time.fixedDeltaTime * (Mathf.Abs(nowPendulum) * Time.fixedDeltaTime / (PendulumFactor * Time.fixedDeltaTime));
        //    if (pendulumDelta < 0.001f)
        //    {
        //        Side *= -1;
        //        returing = true;
        //    }
        //    if (pendulumDelta == )

        //        if (Side > 0)
        //        {
        //            nowPendulum += pendulumDelta;
        //        }
        //        else
        //        {
        //            nowPendulum -= PendulumFactor * Time.fixedDeltaTime;
        //        }

        //    this.transform.rotation = Quaternion.Euler(new Vector3(nowPendulum, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z));
        //}

        public void InitalizeDrop(BonusComponent bonusComponent)
        {

            var bonusSkin = bonusComponent.BonusConfig.Replace("battle\\bonus\\", "battle\\bonusskin\\") + "\\" + bonusComponent.BonusEra + "\\" + bonusComponent.BonusVersion;
            
            var skinResources = ResourcesService.instance.GameAssets.GetDirectory(bonusSkin).FillChildContentToItem();
            var skinCommonResources = ResourcesService.instance.GameAssets.GetDirectory("battle\\bonusskin\\common" + "\\" + bonusComponent.BonusEra + "\\" + bonusComponent.BonusVersion).GetChildFSObject("card").GetContent<ItemCard>();
            this.boxResource = skinResources + skinCommonResources;

            //this.dropBonusPath = this.boxResourceDB.DropBonusPaths[bonusComponent.ConfigPath];
            //this.boxResource = this.boxResourceDB.BattleObjectsResourcesDB[bonusComponent.ConfigPath][dropBonusPath.BoxEraName];


            ParachuteVisualObj.GetComponent<MeshRenderer>().materials.ForEach((Material material) => material.mainTexture = boxResource.GetElement<Texture>("para"));
            //todo: task: need add resources for box parachute

            var boxMesh = boxResource.GetElement<GameObject>("model").GetComponent<MeshFilter>().sharedMesh;
            this.GetComponent<MeshFilter>().mesh = boxMesh;
            //this.GetComponent<BoxCollider>().bounds.Encapsulate(boxMesh.bounds);
            this.GetComponent<BoxCollider>().size = boxMesh.bounds.size;
            this.GetComponent<BoxCollider>().center = boxMesh.bounds.center;
            this.Box.GetComponent<MeshFilter>().mesh = boxMesh;
            //this.Box.GetComponent<BoxCollider>().bounds.Encapsulate(boxMesh.bounds);
            this.Box.GetComponent<BoxCollider>().size = boxMesh.bounds.size;
            this.Box.GetComponent<BoxCollider>().center = boxMesh.bounds.center;
            this.Box.GetComponent<MeshRenderer>().material.mainTexture = boxResource.GetElement<Texture>("texture");
            var newY = boxMesh.bounds.size.y/2 + boxMesh.bounds.center.y;// - this.BoundPoint.transform.localPosition.y;
            ParachuteObj.transform.localPosition = new Vector3(ParachuteObj.transform.localPosition.x, ParachuteObj.transform.localPosition.y + newY, ParachuteObj.transform.localPosition.z);
            this.transform.position = bonusComponent.position.ConvertToUnityVector3Constant007Scaling();
        }

        protected override void OnTriggerEnterManager(Collider other)
        {
            base.OnTriggerEnterManager(other);
            var tankMnager = other.gameObject.GetComponent<IManagableAnchor>();
            if (!localTaken && tankMnager != null && tankMnager.ownerManager<TankManager>().ManagerEntityId == ClientNetworkService.instance.PlayerEntityId)
            {
                var tankPos = tankMnager.ownerManager<TankManager>().Hull.transform.position;
                TaskEx.RunAsync(() => {
                    ClientNetworkService.instance.Socket.emit(new RawDropTakingEvent()
                    {
                        contactPosition = Vector3S.ConvertToVector3SUnScaling(tankPos, Const.ResizeResourceConst),
                        dropEntityId = this.ConnectPoint.instanceId
                    });
                });
                localTaken = true;
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Drop") || other.isTrigger)
            {
                return;
            }
            if (!Landed)
            {
                disablePendulum = true;
                this.constantForce.enabled = false;
                Landed = true;
            }
        }

        protected override void OnTriggerExitManager(Collider other)
        {
            base.OnTriggerExitManager(other);
        }

        public override void AddManager()
        {
            
        }

        protected override void OnStartManager()
        {
            
        }

        protected override void OnAwakeManager()
        {
            
        }

        protected override void OnRemoveManager()
        {
            
            //if(!this.gameObject.activeInHierarchy)
            //{
            //    Destroy(this.gameObject);
            //}
            //this.gameObject.SetActive(false);
        }

        protected override void OnActivateManager()
        {
            
        }

        protected override void OnDeactivateManager()
        {
            
        }

        public override void RemoveManager()
        {
            if(Created)
            {
                var dropEntity = this.ConnectPoint as BonusComponent;
                if (dropEntity.bonusState == BonusState.Taken)
                {
                    this.Taken = true;
                }
                if (dropEntity.bonusState == BonusState.Despawned)
                {
                    this.Despawned = true;
                }
            }
        }
    }
}
