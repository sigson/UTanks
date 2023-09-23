using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using SecuredSpace.Battle.Tank;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Extensions;
using SecuredSpace.Battle;

namespace Assets.ClientCore.CoreImpl.ECS.Systems.Battles
{
    public class BattleInfoUpdaterSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            ComponentsOnChangeCallbacks.Add(HealthComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
                    {
                        tankManager.ExecuteInstruction(() => {
                            tankManager.tankUI.GetComponent<TankUI>().UpdateInfo(entity, component as HealthComponent);
                        }, "Error access tank ui");
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(UsernameComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
                    {
                        tankManager.ExecuteInstruction(() => {
                            tankManager.tankUI.GetComponent<TankUI>().UpdateInfo(entity, component as UsernameComponent);
                        }, "Error access tank ui");
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(UserRankComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);

                    if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
                    {
                        tankManager.ExecuteInstruction(() => {
                            tankManager.tankUI.GetComponent<TankUI>().UpdateInfo(entity, component as UserRankComponent);
                        }, "Error access tank ui");
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(WeaponEnergyComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
                    {
                        tankManager.ExecuteInstruction(() => {
                            tankManager.tankUI.GetComponent<TankUI>().UpdateInfo(entity, component as WeaponEnergyComponent);
                        }, "Error access tank ui");
                    }
                    //ClientInitService.instance.ExecuteInstruction((object Obj) =>
                    //{

                    //    TankManager manager;
                    //    if(ClientInitService.instance.battleManager.BattleTanksDB.TryGetValue(entity.instanceId, out manager))
                    //    {
                    //        manager.tankUI.GetComponent<TankUI>().UpdateInfo(entity, component as WeaponEnergyComponent);
                    //    }
                    //}, null);
                }
            });
            ComponentsOnChangeCallbacks.Add(BattleSimpleInfoComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if(ManagerScope.entityManager.EntityStorage.TryGetValue(ClientNetworkService.instance.PlayerEntityId, out _))
                    {
                        var playerEnt = ManagerScope.entityManager.EntityStorage[ClientNetworkService.instance.PlayerEntityId];
                        if(playerEnt.HasComponent(BattleOwnerComponent.Id) && playerEnt.GetComponent<BattleOwnerComponent>().BattleInstanceId == entity.instanceId)
                        {
                            UIService.instance.ExecuteInstruction((object Obj) =>
                            {
                                //ClientInit.uiManager.battleUIHandler.OnBattleExit();
                                //ClientInit.uiManager.battleUIHandler.BattleUIPrepare(entity);
                                UIService.instance.battleUIHandler.BattleTimeUpdate(entity);
                                UIService.instance.battleUIHandler.UpdateStats(entity);
                                var battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(entity);
                                if(battleManager != null)
                                {
                                    battleManager.battleSimpleInfoComponent = component as BattleSimpleInfoComponent;
                                }
                            }, null);

                        }
                        else
                        {
                            UIService.instance.ExecuteInstruction((object Obj) =>
                            {
                                UIService.instance.BattlesMainLobbyUI.GetComponent<BattleLobbyUIHandler>().UpdateBattleList(entity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo, entity);
                            }, null);
                        }
                    }
                    else
                    {
                        UIService.instance.ExecuteInstruction((object Obj) =>
                        {
                            UIService.instance.BattlesMainLobbyUI.GetComponent<BattleLobbyUIHandler>().UpdateBattleList(entity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo, entity);
                        }, null);
                    }
                    
                }
            });
            ComponentsOnChangeCallbacks.Add(BattleScoreComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    UIService.instance.ExecuteInstruction((object Obj) =>
                    {
                        UIService.instance.battleUIHandler.TeamStatsUpdate(component as BattleScoreComponent);
                    }, null);
                }
            });
            this.Enabled = false;
            this.DelayRunMilliseconds = 200;
        }



        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>()
            {
                //IKeys = {
                //    UserEmailComponent.Id//100% local user
                //},
                //IValues = { 0 }
            }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>()
            {
                //IKeys = {
                //    CreateBattleEvent.Id
                //},
                //IValues = { 0 }
            }.Upd();
        }

        public override void Run(long[] entities)
        {
            
            this.LastEndExecutionTimestamp = DateTime.Now.Ticks;
            this.InWork = false;
        }

        public override void UpdateEventWatcher(ECSEvent eCSEvent)
        {
            
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            return false;
        }
    }
}
