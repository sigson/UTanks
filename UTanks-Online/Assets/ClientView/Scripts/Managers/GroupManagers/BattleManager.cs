using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using Assets.ClientCore.CoreImpl.ECS.Events.Battle;
using SecuredSpace.Battle.Tank;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SecuredSpace.Important.Raven;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.Location;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle.AtomicType;
using SecuredSpace.Battle.Tank.Turret;
using SecuredSpace.CameraControl;
using System.Linq;
using SecuredSpace.Battle.Drop;
using SecuredSpace.Settings;
using SecuredSpace.UI.GameUI;
using UTanksClient.Services;
using UnityEngine.UI;
using SecuredSpace.Battle.Creatures;
using SecuredSpace.Important.TPhysics;
using UTanksClient.Extensions;
using UTanksClient.ECS.Types.Battle.Team;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.ClientControl.Managers;
using UTanksClient.ECS.Components;
using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.UnityExtend.MarchingBytes;

namespace SecuredSpace.Battle
{
    public class BattleManager : IGroupManager<ECSEntity>
    {
        public Dictionary<long, TankManager> BattleTanksDB {
            get
            {
                Dictionary<long, TankManager> result = new Dictionary<long, TankManager>();
                this.ForEach(x => result.Add(x.Key, x.Value.GetComponent<EntityManagersComponent>().Get<TankManager>()));
                return result;
            }
        }
        public Dictionary<long, ICreature> BattleCreatureDB = new Dictionary<long, ICreature>();
        public MapManager MapManager => ((ECSEntity)this.ConnectPoint).GetComponent<EntityManagersComponent>()[typeof(MapManager)] as MapManager;
        
        
        public GameObject spritePrefab;
        public static Material SkyboxMaterial; //for settings, for client only solution :)
        public long NowBattleId => this.ConnectPoint.instanceId;
        public ECSEntity NowBattleEntity => (ECSEntity)this.ConnectPoint;
        public BattleSimpleInfoComponent battleSimpleInfoComponent;
        public long EntityOfCameraFollowing;

        public bool PreLoaded = false;
        public bool Loaded = false;

        protected override void UpdateManager()
        {
            if (PreLoaded && MapManager.MapLoaded)
            {
                var battleLoadedEvent = new BattleLoadedEvent()
                {
                    BattleId = NowBattleId
                };
                PreLoaded = false;
                Loaded = true;
                MapManager.MapLoaded = false;
                UIService.instance.battleUIHandler.BattleUIPrepare(ManagerScope.entityManager.EntityStorage[NowBattleId]);
                UIService.instance.PlayerPanelUI.SetActive(true);
                UIService.instance.LoadingWindowUI.SetActive(false);
                UIService.instance.BattleUI.SetActive(true);
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
                TaskEx.RunAsync(() =>
                {
                    ClientNetworkService.instance.Socket.emit(battleLoadedEvent.PackToNetworkPacket());
                });
            }
            if(!PreLoaded && !Loaded)
            {
                CheckTanksLoaded();
            }
        }

        public void BattleSupplyUIPrepare(ECSEntity playerEntity, BattleComponent battleComponent)
        {
            var battleUI = UIService.instance.battleUIHandler;
            battleUI.Supplies.Values.ForEach(x => Destroy(x.gameObject));
            battleUI.Supplies.Clear();
            if (battleComponent.enablePlayerSupplies)
            {
                var supplies = ConstantService.instance.GetByHeadLibName("supplies");
                supplies = supplies.ToList().OrderBy(x => int.Parse(x.Deserialized["order"]["index"].ToString())).ToArray();
                var battleGarage = playerEntity.GetComponent<UserBattleGarageDBComponent>();
                int counter = 0;
                foreach (var supply in supplies)
                {
                    var findedSupply = battleGarage.battleEquipment.Supplies.Where(x => x.PathName == supply.Path);
                    if (supply.Deserialized["alwaysShow"]["show"].ToString() == "true" || findedSupply.Count() > 0)
                    {
                        var battleIcon = ResourcesService.instance.GameAssets.GetDirectory($"battle\\bonus\\ui\\bonuspanelicon").FillChildContentToItem().GetElement<ItemCard>("card").GetElement<Sprite>(supply.Path);
                        var newSupply = Instantiate(battleUI.SupplyExample, battleUI.SupplyExample.transform.parent);
                        //newSupply.supplyEffectFadeRadialIndicator.sprite = battleIcon;
                        newSupply.supplyEffectFadeRadialIndicator.sprite = battleIcon;
                        newSupply.supplyUsingDelayFadeRadialIndicator.sprite = battleIcon;
                        newSupply.keyCode += counter;
                        newSupply.SupplyConfigPath = supply.Path;
                        newSupply.gameObject.SetActive(true);
                        battleUI.Supplies.Add(supply.Path, newSupply);
                        if (findedSupply.Count() > 0)
                            newSupply.supplyCount.text = findedSupply.ToArray()[0].Count.ToString();
                        else
                            newSupply.supplyCount.text = "0";
                        counter++;
                    }
                }
            }
            else if (battleComponent.enableSupplyDrop)
            {
                var supplies = ConstantService.instance.GetByHeadLibName("supplies");
                supplies = supplies.ToList().OrderBy(x => int.Parse(x.Deserialized["order"]["index"].ToString())).ToArray();
                int counter = 0;
                foreach (var supply in supplies)
                {
                    if (supply.Deserialized["alwaysShow"]["show"].ToString() == "true")
                    {
                        var battleIcon = ResourcesService.instance.GameAssets.GetDirectory($"battle\\bonus\\ui\\bonuspanelicon").FillChildContentToItem().GetElement<ItemCard>("card").GetElement<Sprite>(supply.Path);

                        var newSupply = Instantiate(battleUI.SupplyExample, battleUI.SupplyExample.transform.parent);
                        newSupply.GetComponent<Image>().sprite = battleIcon;
                        newSupply.supplyEffectFadeRadialIndicator.sprite = battleIcon;
                        newSupply.supplyUsingDelayFadeRadialIndicator.sprite = battleIcon;
                        newSupply.keyCode += counter;
                        newSupply.SupplyConfigPath = supply.Path;
                        newSupply.gameObject.SetActive(true);
                        battleUI.Supplies.Add(supply.Path, newSupply);
                        newSupply.GetComponent<BattleSupplyElement>().supplyCount.enabled = false;
                        counter++;
                    }
                }
            }
        }

        public static void LoadedBattleClientAction(ECSEntity entity, Action<object> actionOnLoaded, bool clientEntity = true, bool ignoreLoaded = false)
        {
            LoadedBattleClientAction(entity.instanceId, actionOnLoaded, clientEntity, ignoreLoaded);
        }

        public static void LoadedBattleClientAction(long entity, Action<object> actionOnLoaded, bool clientEntity = true, bool ignoreLoaded = false)
        {
            TaskEx.RunAsync(() => {
                bool BattleFinded = false;
                int exitCounter = 0;
                while (!BattleFinded)
                {
                    BattleManager battleManager = null;
                    if (clientEntity)
                        battleManager = EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>(entity);
                    else
                        battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(entity);
                    if (battleManager != null && (ignoreLoaded || battleManager.Loaded))
                    {
                        BattleFinded = true;
                        battleManager.ExecuteInstruction(() => actionOnLoaded(battleManager));
                    }
                    else
                    {
                        Task.Delay(100).Wait();
                        exitCounter++;
                    }
                    if(exitCounter > 5000)
                    {
                        BattleFinded = true;
                    }
                }
            });
        }

        public void CheckTanksLoaded()
        {
            try
            {
                int tankCount = 0;
                NowBattleEntity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands.Values.ForEach((command) => command.commandPlayers.ForEach((player) => tankCount++));
                if (this.Count == tankCount)
                    PreLoaded = true;
            }
            catch (Exception ex)
            {
                ULogger.Log("Error check tanks\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }


        public void EnterToBattle(ECSEntity player)
        {

        }

        public void SendEventToTank(ECSEntity player, ECSEvent tankEvent)
        {

        }

        public void RebuildTank(ECSEntity player)
        {

        }

        public void UpdateTankInfoStatements(ECSEntity player)
        {

        }

        public void UpdateTankTransformStatements(ECSEntity player, ECSEvent updatingEvent)
        {

        }

        public void SendTurretEvent(ECSEntity player, ECSEvent updatingEvent)
        {

        }


        public void FollowCameraMoveToPosition(WorldPoint worldPoint)
        {

        }

        public void FollowCameraCurveMoveToPosition(WorldPoint worldPoint)
        {

        }

        protected override void OnStartManager()
        {
            UIService.instance.HideAll();
            UIService.instance.BackgroundUI.SetActive(true);
            UIService.instance.PlayerPanelUI.SetActive(false);
            UIService.instance.LoadingWindowUI.SetActive(true);
            var battleComponent = NowBattleEntity.GetComponent<BattleComponent>(BattleComponent.Id);
            NowBattleEntity.GetComponent<EntityManagersComponent>()[typeof(MapManager)] = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.MapBuilder), this.transform).GetComponent<MapManager>();
            MapManager.GetComponent<MapManager>().LoadMap(battleComponent);
            if (!Loaded)
                CheckTanksLoaded();
        }

        protected override void OnAwakeManager()
        {
            
        }

        protected override void OnRemoveManager()
        {
            MapManager.GetComponent<MapManager>().ClearMap();

            ManagerScope.entityManager.EntityStorage[NowBattleId].TryRemoveComponent(BattleCreatureStorageComponent.Id);

            PreLoaded = false;
            Loaded = false;
            UIService.instance.battleUIHandler.OnBattleExit();
            this.MapManager.GetComponent<AudioSource>().Stop();
            ClientInitService.instance.MainCamera.SetActive(true);
            ClientInitService.instance.NowMainCamera = ClientInitService.instance.MainCamera;

            UIService.instance.battleUIHandler.messages.Keys.ForEach((message) => UnityEngine.MonoBehaviour.Destroy(message.gameObject));
            UIService.instance.battleUIHandler.messages.Clear();
            UIService.instance.battleUIHandler.Supplies.ForEach(x=> Destroy(x.Value.gameObject));
            UIService.instance.battleUIHandler.Supplies.Clear();
            UIService.instance.battleUIHandler.KillLogs.ForEach((message) => UnityEngine.MonoBehaviour.Destroy(message.gameObject));
            UIService.instance.battleUIHandler.KillLogs.Clear();
            UIService.instance.battleUIHandler.TeamStatsPanel.SetActive(false);
            UIService.instance.battleUIHandler.LeftTeamGoal.gameObject.SetActive(false);
            UIService.instance.battleUIHandler.LeftTeamGoal.GoalBlock.gameObject.SetActive(false);
            UIService.instance.battleUIHandler.RightTeamGoal.gameObject.SetActive(false);
            UIService.instance.battleUIHandler.RightTeamGoal.GoalBlock.gameObject.SetActive(false);
        }

        public override void ActivateManager()
        {
            
        }

        public override void DeactivateManager()
        {
            
        }

        public override void RemoveManager()
        {
            
        }

        protected override void OnAdd(ECSEntity entity)
        {
            CheckTanksLoaded();
        }

        protected override void OnGet(ECSEntity entity)
        {
            
        }

        protected override void OnSet(ECSEntity entity)
        {
            
        }

        protected override void OnRemove(ECSEntity entity)
        {
            entity.GetComponent<EntityManagersComponent>().Get<TankManager>().RemoveTankFromBattle(entity);
            if (!ClientInitService.instance.CheckEntityIsPlayer(entity))
                ManagerScope.entityManager.OnRemoveEntity(entity);

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
