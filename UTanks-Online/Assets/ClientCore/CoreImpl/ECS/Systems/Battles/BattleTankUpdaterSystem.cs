using Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle;
using Assets.ClientCore.CoreImpl.ECS.Components.Battle.CharacteristicTransformers;
using Assets.ClientCore.CoreImpl.ECS.Components.Battle.Hull;
using SecuredSpace.Battle.Tank;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Extensions;
using UTanksClient.ECS.Components;

namespace Assets.ClientCore.CoreImpl.ECS.Systems.Battles
{
    public class BattleTankUpdaterSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            ComponentsOnChangeCallbacks.Add(HullComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if(ManagerScope.entityManager.EntityStorage.TryGetValue(entity.instanceId, out var playerEntity) &&
                    playerEntity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var manager))
                    {
                        manager.HotRebuildTank(entity);
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(UserBattleGarageDBComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    entity.AddComponent(new RebuildAfterDeathComponent(3f).SetGlobalComponentGroup());
                }
            });
            ComponentsOnChangeCallbacks.Add(DamageTransformerComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if(entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankM))
                    {
                        tankM.ExecuteInstruction(() => {
                            tankM.tankUI.GetComponent<TankUI>().UpdateSupplyInfo(entity);
                            if (ClientInitService.instance.CheckEntityIsPlayer(entity))
                            {
                                UIService.instance.battleUIHandler.Supplies[@"garage\supplies\doubledamage"].GetComponent<BattleSupplyElement>().SetupTimer(Convert.ToSingle((component as TimerComponent).TimeRemaining / 1000));
                            }
                        });
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(ArmorTransformerComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if(entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankM))
                    {
                        tankM.ExecuteInstruction(() => {
                            tankM.tankUI.GetComponent<TankUI>().UpdateSupplyInfo(entity);
                            if (ClientInitService.instance.CheckEntityIsPlayer(entity))
                            {
                                UIService.instance.battleUIHandler.Supplies[@"garage\supplies\doublearmor"].GetComponent<BattleSupplyElement>().SetupTimer(Convert.ToSingle((component as TimerComponent).TimeRemaining / 1000));
                            }
                        });
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(RepairTransformerComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if(entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankM))
                    {
                        tankM.ExecuteInstruction(() => {
                            tankM.tankUI.GetComponent<TankUI>().UpdateSupplyInfo(entity);
                            if (ClientInitService.instance.CheckEntityIsPlayer(entity))
                            {
                                UIService.instance.battleUIHandler.Supplies[@"garage\supplies\aid"].GetComponent<BattleSupplyElement>().SetupTimer(Convert.ToSingle((component as TimerComponent).TimeRemaining / 1000));
                            }
                        });
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(NitroTransformerComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if(entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankM))
                    {
                        tankM.ExecuteInstruction(() => {
                            tankM.tankUI.GetComponent<TankUI>().UpdateSupplyInfo(entity);
                            if (ClientInitService.instance.CheckEntityIsPlayer(entity))
                            {
                                UIService.instance.battleUIHandler.Supplies[@"garage\supplies\nitroboost"].GetComponent<BattleSupplyElement>().SetupTimer(Convert.ToSingle((component as TimerComponent).TimeRemaining / 1000));
                            }
                        });
                    }
                }
            });
            #region turrets

            #endregion
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
