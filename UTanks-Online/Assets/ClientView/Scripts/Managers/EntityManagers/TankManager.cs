using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SecuredSpace.Important.Raven;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.Services;
using SecuredSpace.Battle.Tank.Turret;
using SecuredSpace.Important.Aim;
using SecuredSpace.Battle.Tank.Hull;
using SecuredSpace.UI.GameUI;
using SecuredSpace.ClientControl.DBResources;
using System.Globalization;
using Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle;
using SecuredSpace.Important.TPhysics;
using UTanksClient.Core.Logging;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using UTanksClient.Extensions;
using UTanksClient.ECS.Types.Battle.AtomicType;
using UTanksClient.ECS.Types.Battle.Team;
using SecuredSpace.CameraControl;
using SecuredSpace.Settings;
using UTanksClient.ECS.Components.Battle.Location;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.Effects;
using SecuredSpace.UnityExtend;

namespace SecuredSpace.Battle.Tank
{
    public class TankManager : IEntityManager
    {
        public GameObject MainTankGameObject;
        public GameObject Hull;
        public GameObject HullModel;
        public GameObject Turret;
        public GameObject TurretModel;
        public ITurretVisualController turretVisualController;
        public IHullVisualController hullVisualController;
        public Dictionary<string, GameObject> HullAngleColliders = new Dictionary<string, GameObject>();
        public GameObject tankUI;
        public bool tankUIReleased = false;

        private BattleManager cacheBattleManager = null;
        public BattleManager battleManager
        {
            get
            {
                if(cacheBattleManager == null)
                {
                    cacheBattleManager = EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>(this.ConnectPoint.instanceId);
                }
                return cacheBattleManager;
            }
        }

        #region Links

        public GameObject TankFrictionCollidersObject;
        public GameObject TankBoundsObject;
        public GameObject HullCenterCollider;
        public GameObject HullBalanceCollider;
        public GameObject HullVisibleModel;
        public GameObject HullAngleColliderHeader;

        public RaycastAIM raycastAIM { 
            get
            {
                if(turretVisualController.raycastAIM == null)
                {
                    turretVisualController.raycastAIM = Turret.GetComponent<RaycastAIM>();
                }
                return turretVisualController.raycastAIM;
            }
            set => turretVisualController.raycastAIM = value; 
        }
        public CloseDistanceAIM closeDistanceAIM { get => turretVisualController.closeDistanceAIM; set => turretVisualController.closeDistanceAIM = value; }
        public List<GameObject> MuzzlePoints { get => turretVisualController.MuzzlePoints; set => turretVisualController.MuzzlePoints = value; }
        public GameObject MuzzlePointExample => turretVisualController.MuzzlePointExample;
        public GameObject MuzzlePoint { get => turretVisualController.MuzzlePoint; set => turretVisualController.MuzzlePoint = value; }
        public int selectedMuzzlePoint { get => turretVisualController.selectedMuzzlePoint; set => turretVisualController.selectedMuzzlePoint = value; }
        public GameObject MuzzleCheckPoint { get => turretVisualController.MuzzleCheckPoint; set => turretVisualController.MuzzleCheckPoint = value; }
        public ItemCard TurretResources { get => turretVisualController.TurretResources; set => turretVisualController.TurretResources = value; }
        public ItemCard HullResources { get => hullVisualController.HullResources; set => hullVisualController.HullResources = value; }
        public ConfigObj turretGameplayObj { get => turretVisualController.gameplayConfig; set => turretVisualController.gameplayConfig = value; }
        
        public ConfigObj hullGameplayObj { get => hullVisualController.gameplayConfig; set => hullVisualController.gameplayConfig = value; }

        #endregion

        public ITurret turretManager => turretVisualController.turretManager;
        public HullGhostChecker hullGhostChecker;
        public HullManager hullManager;
        public AudioAnchor hullAudioSource => hullManager.hullAudioSource;
        public AudioAnchor turretAudioSource => turretManager.weaponAudio;

        public AnimationScript tankExplosion;
        public AnimationScript tankExplosionSmoke;
        public AnimationScript tankExplosionShokWave;

        public long playerCommandEntityId;
        public bool TestingMode = false;
        public bool Ghost = false;
        public bool checkGhost = false;
        public bool deadTank = false;
        public bool firstSpawn = true;
        public bool TestRebuildHullAngleColliders = false;
        public string TeamColor;
        public Color NicknameColor;
        private float destroyUpDirection = 410000;
        //============================

        private CameraControl.CameraController cameraController;
        protected override void FixedUpdateManager()
        {
            if (TestRebuildHullAngleColliders)
            {
                HullAngleColliders.Clear();
                for (int i = 0; i < hullVisualController.HullAngleColliderHeader.transform.childCount; i++)
                {
                    var child = hullVisualController.HullAngleColliderHeader.transform.GetChild(i);
                    HullAngleColliders.Add(child.name, child.gameObject);
                }
                TestRebuildHullAngleColliders = false;
            }
            if(firstSpawn)
            {
                if (hullManager.chassisManager.chassisNode != null)
                {
                    hullManager.chassisManager.chassisNode.track.LeftTrack.SetRayñastLayerMask(LayerMask.GetMask("Default"));
                    hullManager.chassisManager.chassisNode.track.RightTrack.SetRayñastLayerMask(LayerMask.GetMask("Default"));
                    firstSpawn = false;
                }
            }
            if (!ClientInitService.instance.LockInput && Input.GetKeyDown(KeyCode.Delete))
            {
                ClientNetworkService.instance.Socket.emit(new SelfDestructionRequestEvent().PackToNetworkPacket());
            }
            if (!ClientInitService.instance.LockInput && Input.GetKeyUp(KeyCode.F) && ClientInitService.instance.CheckEntityIsPlayer(ManagerEntity) && !ManagerEntity.HasComponent(SpectatorSwitchCooldownComponent.Id))
            {
                if(cameraController == null)
                    cameraController = ClientInitService.instance.NowMainCamera.transform.parent.gameObject.GetComponent<CameraControl.CameraController>();
                cameraController.Follow = !cameraController.Follow;
                cameraController.SpectatorMode = !cameraController.SpectatorMode;
                ManagerEntity.AddComponent(new SpectatorSwitchCooldownComponent(0.3f).SetGlobalComponentGroup());
            }
        }

        public static TankManager Create(ECSEntity playerEntity, ECSEntity battleEntity)
        {
            var battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(battleEntity);
            var PlayerTank = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.Tank), battleManager.MapManager.PlayersSpace.transform);
            var tankManager = PlayerTank.GetComponent<TankManager>();

            tankManager.ManagerEntity = playerEntity;
            battleEntity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands.ForEach((command) =>
            {
                if (command.Value.commandPlayers.Keys.ToList().Contains(tankManager.ManagerEntityId))
                {
                    tankManager.playerCommandEntityId = command.Key;
                    tankManager.TeamColor = command.Value.TeamColor;
                    var teamColor = ColorEx.ToColor(TeamColors.colors[tankManager.TeamColor]);
                    var counterOffset = TeamColors.elementColorOffsets["nicknameColor"];
                    tankManager.NicknameColor = Color.Lerp(teamColor, ColorEx.ToColor(counterOffset.Item1), counterOffset.Item2);
                }
            });
            tankManager.RebuildTank(playerEntity);
            var turretRot = tankManager.Turret.GetComponent<TurretRotaion>();
            turretRot.turretBaseSound.audioStorage.Add(tankManager.TurretResources.GetElement<AudioSourceSetting>("turret").UpdateSettings("turret", true, 1f));
            turretRot.turretBaseSound.Build();
            if (ClientInitService.instance.CheckEntityIsPlayer(tankManager.ManagerEntity))
            {
                turretRot.Movable = true;
                tankManager.Hull.GetComponent<TankChassisManager>().TankMovable = true;
                var followingCamera = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.BattleCamera), battleManager.MapManager.PlayersSpace.transform);
                followingCamera.GetComponent<CameraController>().ownerManagerSpace = battleManager.MapManager;
                followingCamera.GetComponent<CameraController>().target = tankManager.Turret.transform;
                tankManager.gameObject.SetActive(false);
                ClientInitService.instance.MainCamera.SetActive(false);
                ClientInitService.instance.NowMainCamera = followingCamera.transform.GetChild(0).gameObject;
                ClientInitService.instance.anchorHandler.UpdateAnchors<PostProcessAnchor>((anchor) => true);
                UIService.instance.BackgroundUI.SetActive(false);
                battleManager.BattleSupplyUIPrepare(playerEntity, battleEntity.GetComponent<BattleComponent>());
            }

            if (!tankManager.tankUIReleased)
            {
                tankManager.tankUI = Instantiate(ResourcesService.instance.GetPrefab("TankUI"), UIService.instance.BattleUI.transform);
                tankManager.tankUI.transform.SetAsFirstSibling();
                tankManager.tankUI.GetComponent<Canvas>().enabled = true;
                tankManager.tankUI.GetComponent<TankUI>().parentTankManager = tankManager;
                tankManager.tankUIReleased = true;
            }

                tankManager.ManagerEntity = playerEntity;

            battleEntity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands.ForEach((command) =>
            {
                if (command.Value.commandPlayers.Keys.ToList().Contains(tankManager.ManagerEntityId))
                {
                    tankManager.playerCommandEntityId = command.Key;
                    tankManager.TeamColor = command.Value.TeamColor;
                    var teamColor = ColorEx.ToColor(TeamColors.colors[tankManager.TeamColor]);
                    var counterOffset = TeamColors.elementColorOffsets["nicknameColor"];
                    tankManager.NicknameColor = Color.Lerp(teamColor, ColorEx.ToColor(counterOffset.Item1), counterOffset.Item2);
                }
            });
            tankManager.RebuildTank(playerEntity);
            tankManager.Hull.transform.localPosition = playerEntity.GetComponent<WorldPositionComponent>(WorldPositionComponent.Id).WorldPoint.Position.ConvertToUnityVector3Constant007Scaling();
            var rotation = playerEntity.GetComponent<WorldPositionComponent>(WorldPositionComponent.Id).WorldPoint.Rotation.ConvertToUnityVector3();
            tankManager.Hull.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

            Assets.ClientCore.CoreImpl.ECS.Types.Battle.Command playerCommand = null;
            Assets.ClientCore.CoreImpl.ECS.Types.Battle.CommandPlayers commandPlayer = null;
            foreach (var command in battleEntity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands)
            {
                foreach (var player in command.Value.commandPlayers)
                {
                    if (player.Key == playerEntity.instanceId)
                    {
                        playerCommand = command.Value;
                        commandPlayer = player.Value;
                    }
                }
            }
            if (commandPlayer != null)
                tankManager.tankUI.GetComponent<TankUI>().UpdateInfo(commandPlayer);

            if (!playerEntity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out _))
            {
                playerEntity.GetComponent<EntityManagersComponent>().Add(typeof(TankManager), tankManager);
            }
            tankManager.isPrefabScript = false;
            tankManager.GetComponentsInChildren<IManagableAnchor>().ForEach(x => x.ownerManagerSpace = tankManager);
            return tankManager;
        }

        public void ProcessedEvent(ECSEvent ecsEvent)
        {
#if AggressiveLog
            List<(string, object)> variableLogger = new List<(string, object)>();
            try
#endif
            {
                if (ecsEvent.EntityOwnerId != ClientNetworkService.instance.PlayerEntityId)
                {
                    (battleManager[ecsEvent.EntityOwnerId].GetComponent<EntityManagersComponent>()[typeof(TankManager)] as TankManager).turretManager.TurretInfluenceEvent(ManagerScope.entityManager.EntityStorage[ecsEvent.EntityOwnerId], ecsEvent);
                }
            }
#if AggressiveLog
            catch (Exception ex)
            {
                string variableChecker = "";
                variableLogger.ForEach(x => variableChecker += x.Item1 + " = " + x.Item2 + "\n");
                ULogger.Error("TankProcessedEvent\n" + ex.Message + "\n" + ex.StackTrace + "\n" + variableChecker);
            }
#endif
        }
        public void ShotKickback(ECSEntity playerEntity)
        {
            
        }

        public void SpawnTank(ECSEntity player, WorldPoint spawnpoint)
        {
            //return;
#if AggressiveLog
            List<(string, object)> variableLogger = new List<(string, object)>();
            try
#endif
            {
                var tankManag = this;
                tankManag.Hull.transform.localPosition = spawnpoint.Position.ConvertToUnityVector3Constant007Scaling();
                tankManag.Hull.GetComponent<Rigidbody>().velocity = Vector3.zero;
                tankManag.Hull.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                var rotation = spawnpoint.Rotation.ConvertToUnityVector3();
                tankManag.Hull.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                //tankManag.SetupColormap(tankManag.ColormapResources);
                tankManag.Turret.transform.localRotation = Quaternion.identity;
                if (tankManag.ManagerEntityId == ClientNetworkService.instance.PlayerEntityId)
                {
                    tankManag.hullManager.chassisManager.TankMovable = true;
                    tankManag.Turret.GetComponent<TurretRotaion>().Movable = true;
                }
                hullVisualController.SetupColormap(hullVisualController.ColormapResources);
                turretVisualController.SetupColormap(turretVisualController.ColormapResources);
                tankManag.hullManager.chassisManager.StopTank = false;
                tankManag.gameObject.SetActive(true);
            }
#if AggressiveLog
            catch (Exception ex)
            {
                string variableChecker = "";
                variableLogger.ForEach(x => variableChecker += x.Item1 + " = " + x.Item2 + "\n");
                ULogger.Error("SpawnTank\n" + ex.Message + "\n" + ex.StackTrace + "\n" + variableChecker);
            }
#endif
        }

        public void HitKick(ECSEntity playerEntity)
        {

        }

        public void RemoveTankFromBattle(ECSEntity playerEntity)
        {
            this.turretManager.RemoveTurret(playerEntity);
            Destroy(this.tankUI);
            Destroy(this.gameObject);
        }

        public void DestroyTank(ECSEntity playerEntity)
        {
#if AggressiveLog
            List<(string, object)> variableLogger = new List<(string, object)>();
            try
#endif
            {
                hullVisualController.SetupColormap(hullVisualController.DeadColormapResources);
                turretVisualController.SetupColormap(turretVisualController.DeadColormapResources);
                this.Hull.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up * destroyUpDirection, this.Hull.transform.position);
                this.Hull.GetComponent<TankChassisManager>().TankMovable = false;
                this.Turret.GetComponent<TurretRotaion>().Movable = false;
                this.Hull.GetComponent<TankChassisManager>().StopTank = true;
                this.turretManager.DestroyTurret(playerEntity);
                this.deadTank = true;
                this.hullAudioSource.audioManager.FadeAll(new List<string>() { "audio_tank_explosion" });
                this.hullAudioSource.audioManager.PlayBlock(new List<string>() { "audio_tank_explosion" });
                this.tankExplosionShokWave.transform.parent.gameObject.SetActive(true);
                this.tankExplosion.transform.position = this.Turret.transform.position;
                this.tankExplosion.Play();
                this.tankExplosionSmoke.transform.position = this.Turret.transform.position;
                this.tankExplosionSmoke.Play();

                RaycastHit raycastHit;
                if (this.hullManager.chassisManager.isTankOnGround && Physics.Raycast(this.Hull.transform.position, Vector3.down, out raycastHit, Const.MaxUnityFloatValue, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                {
                    this.tankExplosionShokWave.transform.position = Vector3.MoveTowards(this.Hull.transform.position, raycastHit.point, Vector3.Distance(this.Hull.transform.position, raycastHit.point) * 0.85f);
                    this.tankExplosionShokWave.transform.rotation = this.Hull.transform.rotation;
                    this.tankExplosionShokWave.transform.Rotate(new Vector3(270f, 0, 0));
                    this.tankExplosionShokWave.Play();
                }

                
                hullVisualController.TankBoundsObject.SetActive(false);
                StartCoroutine("AwaitToHide");
            }
#if AggressiveLog
            catch (Exception ex)
            {
                string variableChecker = "";
                variableLogger.ForEach(x => variableChecker += x.Item1 + " = " + x.Item2 + "\n");
                ULogger.Error("DestroyTank\n" + ex.Message + "\n" + ex.StackTrace + "\n" + variableChecker);
            }
#endif
        }

        public IEnumerator AwaitToHide()
        {
            yield return new WaitForSeconds(2);
            this.gameObject.SetActive(false);
            this.tankExplosionShokWave.transform.parent.gameObject.SetActive(false);
        }

        public void RebuildTank(ECSEntity playerEntity)
        {
            var selectedEquip = playerEntity.GetComponent<UserBattleGarageDBComponent>(UserBattleGarageDBComponent.Id).battleEquipment;
            if(turretVisualController != null)
                turretVisualController.RemoveController();
            if (hullVisualController != null)
                hullVisualController.RemoveController();

            turretVisualController = ITurretVisualController.InitializeController(TurretModel, selectedEquip, false, this);
            turretVisualController.ownerManagerSpace = this;
            hullVisualController = IHullVisualController.InitializeController(HullModel, selectedEquip, false, this);
            hullVisualController.ownerManagerSpace = this;
            ITankVisualController.CombineTankParts(turretVisualController, hullVisualController);
            hullVisualController.parentTankManager.hullManager.Initialize(this, this.ManagerEntity);
            hullVisualController.parentTankManager.hullManager.RebuildTank(this.ManagerEntity);

            this.hullAudioSource.audioStorage.Clear();
            this.hullAudioSource.audioStorage.Add(this.HullResources.GetElement<AudioSourceSetting>("audio_engineidle").UpdateSettings("audio_engineidle", false, 1f));
            this.hullAudioSource.audioStorage.Add(this.HullResources.GetElement<AudioSourceSetting>("audio_move").UpdateSettings("audio_move", true, 1f));
            this.hullAudioSource.audioStorage.Add(this.HullResources.GetElement<AudioSourceSetting>("audio_move_start").UpdateSettings("audio_move_start", false, 1f));
            this.hullAudioSource.audioStorage.Add(this.HullResources.GetElement<AudioSourceSetting>("audio_engineidle_loop").UpdateSettings("audio_engineidle_loop", true, 1f));
            this.hullAudioSource.audioStorage.Add(this.HullResources.GetElement<AudioSourceSetting>("audio_tank_explosion").UpdateSettings("audio_tank_explosion", false, 1f));
            this.hullAudioSource.Build();

            #region oldvariant
            //////#if AggressiveLog
            //////            List<(string, object)> variableLogger = new List<(string, object)>();
            //////            try
            //////#endif
            //////            {
            //////                var selectedEquip = playerEntity.GetComponent<UserBattleGarageDBComponent>(UserBattleGarageDBComponent.Id).battleEquipment;

            //////                var materials = ResourcesService.instance.GameAssets.GetDirectory("garage\\skin").GetChildFSObject("card").GetContent<ItemCard>();

            //////                HullResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Hulls[0].PathName + "\\" + selectedEquip.Hulls[0].Grade.ToString()).FillChildContentToItem();
            //////                //HullResources += materials;
            //////                TurretResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Turrets[0].PathName + "\\" + selectedEquip.Turrets[0].Grade.ToString()).GetChildFSObject("card").GetContent<ItemCard>();

            //////                //TurretResources += materials;
            //////                //TurretResources = ClientInit.ItemResourcesDBOld.ItemsResourcesDB[selectedEquip.Turrets[0].PathName][selectedEquip.Turrets[0].Grade.ToString()];
            //////                ColormapResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Colormaps[0].PathName).FillChildContentToItem();
            //////                //ColormapResources = ClientInit.ItemResourcesDBOld.ItemsResourcesDB[selectedEquip.Colormaps[0].PathName].Values.ToList()[0];
            //////                DeadColormapResources = ResourcesService.instance.GameAssets.GetDirectory("garage\\colormap\\dead").FillChildContentToItem();
            //////                turretGameplayObj = ConstantService.instance.ConstantDB["battle\\weapon" + selectedEquip.Turrets[0].PathName.Substring(selectedEquip.Turrets[0].PathName.LastIndexOf("\\"))];
            //////                hullGameplayObj = ConstantService.instance.ConstantDB["battle\\tank" + selectedEquip.Hulls[0].PathName.Substring(selectedEquip.Hulls[0].PathName.LastIndexOf("\\"))];
            //////                hullSkinConfig = ConstantService.instance.GetByConfigPath(selectedEquip.Hulls[0].Skins[0].SkinPathName);
            //////                turretSkinConfig = ConstantService.instance.GetByConfigPath(selectedEquip.Turrets[0].Skins[0].SkinPathName);
            //////                colormapSkinConfig = ConstantService.instance.GetByConfigPath(selectedEquip.Colormaps[0].PathName);
            //////                if (!hullSkinConfig.Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
            //////                {
            //////                    HullSkinResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Hulls[0].Skins[0].SkinPathName + "\\" + selectedEquip.Hulls[0].Grade.ToString()).FillChildContentToItem();
            //////                    //HullSkinResources = ClientInit.ItemResourcesDBOld.ItemsResourcesDB[selectedEquip.Hulls[0].PathName][selectedEquip.Hulls[0].Skins[0].SkinPathName];
            //////                }
            //////                else
            //////                {
            //////                    HullSkinResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Hulls[0].Skins[0].SkinPathName).FillChildContentToItem();
            //////                }
            //////                if (!turretSkinConfig.Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
            //////                {
            //////                    TurretSkinResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Turrets[0].Skins[0].SkinPathName + "\\" + selectedEquip.Turrets[0].Grade.ToString()).FillChildContentToItem();
            //////                }
            //////                else
            //////                {
            //////                    TurretSkinResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Turrets[0].Skins[0].SkinPathName).FillChildContentToItem();
            //////                }
            //////                HullSkinResources += materials;
            //////                TurretSkinResources += materials;


            //////                for (int i = 1; i < Turret.transform.childCount; i++)
            //////                    Destroy(Turret.transform.GetChild(i).gameObject);
            //////                for (int i = 1; i < HullAngleColliderHeader.transform.childCount; i++)
            //////                    Destroy(HullAngleColliderHeader.transform.GetChild(i).gameObject);
            //////                for (int i = 1; i < HullBalanceCollider.transform.childCount; i++)
            //////                    Destroy(HullBalanceCollider.transform.GetChild(i).gameObject);
            //////                HullBalanceCollider.GetComponents<Collider>().ForEach(x => Destroy(x));
            //////                TankBoundsObject.GetComponents<Collider>().ForEach(x => Destroy(x));
            //////                MuzzlePoints.Clear();
            //////                #region buildVisualTank

            //////                //var playerTurretPrefab = TurretResources.Models[turretSkinConfig.Deserialized["modelsPathName"].ToList()[0]["model"].ToString()];
            //////                var playerTurretFullModel = TurretSkinResources.GetElement<GameObject>("model");
            //////                var playerTurretModel = playerTurretFullModel.GetComponent<MeshFilter>().sharedMesh;

            //////                //var playerHullPrefab = HullResources.Models[hullSkinConfig.Deserialized["modelsPathName"].ToList()[0]["model"].ToString()];
            //////                var playerHullFullModel = HullSkinResources.GetElement<GameObject>("model");
            //////                var playerHullModel = playerHullFullModel.GetComponent<MeshFilter>().sharedMesh;


            //////                Hull.GetComponent<MeshFilter>().mesh = Instantiate(playerHullModel);
            //////                Hull.GetComponent<MeshCollider>().sharedMesh = Hull.GetComponent<MeshFilter>().mesh;

            //////                HullVisibleModel.GetComponent<MeshFilter>().mesh = Hull.GetComponent<MeshFilter>().mesh;
            //////                HullVisibleModel.GetComponent<MeshCollider>().sharedMesh = Hull.GetComponent<MeshFilter>().mesh;
            //////                Turret.GetComponent<MeshFilter>().mesh = Instantiate(playerTurretModel);
            //////                var boundsCollider = Hull.AddComponent(typeof(BoxCollider)) as BoxCollider;
            //////                boundsCollider.isTrigger = true;
            //////                Hull.GetComponent<TankChassisManager>().MainBoundsCollider = boundsCollider;
            //////                TankFrictionCollidersObject.GetComponent<MeshFilter>().mesh = Hull.GetComponent<MeshFilter>().mesh;
            //////                TankFrictionCollidersObject.GetComponent<MeshCollider>().sharedMesh = Hull.GetComponent<MeshFilter>().mesh;
            //////                TankBoundsObject.GetComponent<MeshFilter>().mesh = Hull.GetComponent<MeshFilter>().mesh;
            //////                var boundBox = TankBoundsObject.AddComponent<BoxCollider>();
            //////                boundBox.size = Hull.GetComponent<MeshFilter>().mesh.bounds.size;
            //////                boundBox.center = Hull.GetComponent<MeshFilter>().mesh.bounds.center;
            //////                boundBox.size = new Vector3(boundBox.size.x, boundBox.size.y / 2, boundBox.size.z);
            //////                var balanceBox = HullBalanceCollider.AddComponent<BoxCollider>();

            //////                balanceBox.size = new Vector3(
            //////                    hullGameplayObj.GetObject<string>($"balanceColliderConfig\\size\\x").FastFloat(), hullGameplayObj.GetObject<string>($"balanceColliderConfig\\size\\y").FastFloat(), hullGameplayObj.GetObject<string>($"balanceColliderConfig\\size\\z").FastFloat());

            //////                //balanceBox.size = new Vector3(float.Parse(hullGameplayObj.Deserialized["balanceColliderConfig"]["size"]["x"].ToString(), CultureInfo.InvariantCulture), float.Parse(hullGameplayObj.Deserialized["balanceColliderConfig"]["size"]["y"].ToString(), CultureInfo.InvariantCulture), float.Parse(hullGameplayObj.Deserialized["balanceColliderConfig"]["size"]["z"].ToString(), CultureInfo.InvariantCulture));
            //////                balanceBox.center = new Vector3(float.Parse(hullGameplayObj.Deserialized["balanceColliderConfig"]["center"]["x"].ToString(), CultureInfo.InvariantCulture), float.Parse(hullGameplayObj.Deserialized["balanceColliderConfig"]["center"]["y"].ToString(), CultureInfo.InvariantCulture), float.Parse(hullGameplayObj.Deserialized["balanceColliderConfig"]["center"]["z"].ToString(), CultureInfo.InvariantCulture));

            //////                normalHullMaterial = Instantiate(HullSkinResources.GetElement<Material>("HullMaterial"));
            //////                transparentHullMaterial = Instantiate(HullSkinResources.GetElement<Material>("HullMaterialTransparent"));
            //////                normalTrackMaterial = Instantiate(HullSkinResources.GetElement<Material>("HullTrack"));
            //////                transparentTrackMaterial = Instantiate(HullSkinResources.GetElement<Material>("HullTrackTransparent"));
            //////                var hullMaterials = new List<Material>() { normalHullMaterial, normalTrackMaterial, transparentHullMaterial, transparentTrackMaterial };
            //////                hullMaterials.ForEach((Material material) => {
            //////                    //material.SetTexture("_Colormap", ColormapResources.GetElement<Texture2D>("image"));
            //////                    material.SetTexture("_Details", HullSkinResources.GetElement<Texture2D>("details"));
            //////                    material.SetTexture("_Lightmap", HullSkinResources.GetElement<Texture2D>("lightmap"));
            //////                });
            //////                HullVisibleModel.GetComponent<MeshRenderer>().materials = hullMaterials.GetRange(0, 2).ToArray();
            //////                normalTurretMaterial = Instantiate(TurretSkinResources.GetElement<Material>("TurretMaterial"));
            //////                transparentTurretMaterial = Instantiate(TurretSkinResources.GetElement<Material>("TurretMaterialTransparent"));
            //////                var turretMaterials = new List<Material> { normalTurretMaterial, transparentTurretMaterial };
            //////                turretMaterials.ForEach((Material material) => {
            //////                    //material.SetTexture("_Colormap", ColormapResources.GetElement<Texture2D>("image"));
            //////                    material.SetTexture("_Details", TurretSkinResources.GetElement<Texture2D>("details"));
            //////                    material.SetTexture("_Lightmap", TurretSkinResources.GetElement<Texture2D>("lightmap"));
            //////                });
            //////                Turret.GetComponent<MeshRenderer>().materials = turretMaterials.GetRange(0, 1).ToArray();

            //////                HullVisibleModel.GetComponent<ColormapScript>().Setup(this.colormapSkinConfig, ColormapResources, true);
            //////                this.Turret.GetComponent<ColormapScript>().Setup(this.colormapSkinConfig, ColormapResources, true);

            //////                Vector3 mountPosition = Vector3.zero;
            //////                var hullResPrefab = playerHullFullModel;
            //////                for (int i = 0; i < hullResPrefab.transform.childCount; i++)
            //////                {
            //////                    if (hullResPrefab.transform.GetChild(i).name.Contains("mount"))
            //////                    {
            //////                        mountPosition = hullResPrefab.transform.GetChild(i).transform.localPosition;
            //////                    }
            //////                }
            //////                var cachePosition = Hull.transform.localPosition;
            //////                Hull.transform.localPosition = Vector3.zero;
            //////                Turret.transform.localPosition = Hull.transform.localPosition + mountPosition;
            //////                this.Turret.transform.localPosition = new Vector3(this.Turret.transform.localPosition.x, this.Turret.transform.localPosition.y + HullVisibleModel.transform.localPosition.y, this.Turret.transform.localPosition.z);
            //////                Hull.transform.localPosition = cachePosition;
            //////                var turretResPrefab = playerTurretFullModel;
            //////                for (int i = 0; i < turretResPrefab.transform.childCount; i++)
            //////                {
            //////                    if (turretResPrefab.transform.GetChild(i).name.Contains("muzzle"))
            //////                    {
            //////                        var muzzlePoint = Instantiate(MuzzlePointExample, MuzzlePointExample.transform.parent);
            //////                        MuzzlePoints.Add(muzzlePoint);
            //////                        MuzzlePoint = muzzlePoint;
            //////                        MuzzlePoint.SetActive(true);
            //////                        selectedMuzzlePoint = MuzzlePoints.Count - 1;
            //////                        MuzzleCheckPoint = MuzzlePoint.transform.GetChild(0).gameObject;
            //////                        MuzzlePoint.transform.localPosition = turretResPrefab.transform.GetChild(i).transform.localPosition;
            //////                        closeDistanceAIM = MuzzlePoint.GetComponent<CloseDistanceAIM>();
            //////                    }
            //////                }
            //////                #endregion
            //////                var turretScripts = Instantiate(TurretResources.GetElement<GameObject>("script"), Turret.transform);
            //////                if (!tankUIReleased)
            //////                {
            //////                    tankUI = Instantiate(tankUI, UIService.instance.BattleUI.transform);
            //////                    tankUI.GetComponent<Canvas>().enabled = true;
            //////                    tankUI.GetComponent<TankUI>().parentTankManager = this;
            //////                    tankUIReleased = true;
            //////                }


            //////                var battleEntity = ManagerScope.entityManager.EntityStorage[playerEntity.GetComponent<BattleOwnerComponent>().BattleInstanceId];
            //////                Assets.ClientCore.CoreImpl.ECS.Types.Battle.Command playerCommand = null;
            //////                Assets.ClientCore.CoreImpl.ECS.Types.Battle.CommandPlayers commandPlayer = null;
            //////                foreach (var command in battleEntity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands)
            //////                {
            //////                    foreach (var player in command.Value.commandPlayers)
            //////                    {
            //////                        if (player.Key == playerEntity.instanceId)
            //////                        {
            //////                            playerCommand = command.Value;
            //////                            commandPlayer = player.Value;
            //////                        }
            //////                    }
            //////                }
            //////                if (commandPlayer != null)
            //////                    tankUI.GetComponent<TankUI>().UpdateInfo(commandPlayer);
            //////                turretManager = turretScripts.GetComponent(typeof(ITurret)) as ITurret;
            //////                turretManager.Initialize(this, playerEntity);
            //////                hullManager.Initialize(this, playerEntity);
            //////                hullManager.RebuildTank(playerEntity);
            //////            }
            //////#if AggressiveLog
            //////            catch (Exception ex)
            //////            {
            //////                string variableChecker = "";
            //////                variableLogger.ForEach(x => variableChecker += x.Item1 + " = " + x.Item2 + "\n");
            //////                ULogger.Error("RebuildTank\n" + ex.Message + "\n" + ex.StackTrace + "\n" + variableChecker);
            //////            }
            //////#endif
            #endregion
        }

        public void EnableGhostTankState()
        {
            hullVisualController.SetGhostMode(true);
            turretVisualController.SetGhostMode(true);
            TankBoundsObject.SetActive(false);
            Hull.layer = LayerMask.NameToLayer("GhostTank");
            hullManager.chassisManager.currentLayerMask = LayerMask.GetMask("Default");
            if(hullManager.chassisManager.chassisNode != null) //fix first spawn error
            {
                hullManager.chassisManager.chassisNode.track.LeftTrack.SetRayñastLayerMask(LayerMask.GetMask("Default"));
                hullManager.chassisManager.chassisNode.track.RightTrack.SetRayñastLayerMask(LayerMask.GetMask("Default"));
            }
            else
            {
                this.firstSpawn = true;
            }
            Ghost = true;
            checkGhost = true;
            this.deadTank = false;

        }

        public void DisableGhostTankState()
        {
            Ghost = false;
            hullGhostChecker.StartCheck();
        }

        public void DisableGhostTankAfterCheck()
        {
            checkGhost = false;
            hullVisualController.SetGhostMode(false);
            turretVisualController.SetGhostMode(false);
            TankBoundsObject.SetActive(true);
            Hull.layer = LayerMask.NameToLayer("Tank");
            hullManager.chassisManager.currentLayerMask = LayerMask.GetMask("Default", "TankBounds");
            hullManager.chassisManager.chassisNode.track.LeftTrack.SetRayñastLayerMask(LayerMask.GetMask("Default", "TankBounds"));
            hullManager.chassisManager.chassisNode.track.RightTrack.SetRayñastLayerMask(LayerMask.GetMask("Default", "TankBounds"));
        }
        public void HotRebuildTank(ECSEntity playerEntity)
        {
            turretManager.HotRebuildTank(playerEntity);
            hullManager.HotRebuildTank(playerEntity);
        }

        protected override void OnStartManager()
        {
            
        }

        protected override void OnAwakeManager()
        {
            
        }

        protected override void OnRemoveManager()
        {
            
        }

        protected override void OnActivateManager()
        {
            
        }

        protected override void OnDeactivateManager()
        {
            
        }

        public override void AddManager()
        {
            
        }
    }

}