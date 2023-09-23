using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using Assets.ClientCore.CoreImpl.ECS.Events.Garage;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using SecuredSpace.Battle.Tank;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using UTanksClient.ECS.Components.Battle.Bonus;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.Components.Battle.Round;
using UTanksClient.ECS.Components.Battle.Tank;
using UTanksClient.ECS.Components.Battle.Team;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle;
using UTanksClient.ECS.Events.Battle.BonusEvents;
using UTanksClient.ECS.Events.Battle.TankEvents;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.ECS.Templates.Battle;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.AtomicType;
using UTanksClient.ECS.Types.Battle.Bonus;
using UTanksClient.Extensions;
using UTanksClient.Network.NetworkEvents.FastGameEvents;
using SecuredSpace.Battle;

namespace UTanksClient.ECS.Systems.Battles
{
    public class BattlesSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(KillEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    KillEntity(Event as KillEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(HitEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    Hit(Event as HitEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(ShotEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    Shot(Event as ShotEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(StartShootingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    StartShooting(Event as StartShootingEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(StartChargingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    StartCharging(Event as StartChargingEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(StartAimingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    StartAiming(Event as StartAimingEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(EndShootingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    EndShooting(Event as EndShootingEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(MoveCommandEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    MoveCommand(Event as MoveCommandEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(SupplyUsedEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    SupplyUsed(Event as SupplyUsedEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            
            
            SystemEventHandler.Add(EnterToBattleEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    EnterToBattle(Event as EnterToBattleEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(LeaveFromBattleEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    ExitFromBatle(Event as LeaveFromBattleEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(BonusTakingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    BonusTaking(Event as BonusTakingEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(UpdateSupplyCountEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    UpdateSupplyCount(Event as UpdateSupplyCountEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            ComponentsOnChangeCallbacks.Add(RoundUserStatisticsComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var manager))
                    {
                        manager.ExecuteInstruction(() => {
                            var stats = component as RoundUserStatisticsComponent;
                            var battle = ManagerScope.entityManager.EntityStorage[entity.GetComponent<BattleOwnerComponent>().BattleInstanceId];
                                var playerObj = battle.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands[manager.playerCommandEntityId].commandPlayers[entity.instanceId];//error
                                battle.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands[manager.playerCommandEntityId].commandPlayers[entity.instanceId] = new Assets.ClientCore.CoreImpl.ECS.Types.Battle.CommandPlayers
                                {
                                    Username = stats.Nickname,
                                    Death = stats.Deaths,
                                    Score = stats.ScoreWithoutBonuses,
                                    EntityId = playerObj.EntityId,
                                    Killed = stats.Kills,
                                    Rank = stats.Rank,
                                    Place = stats.Place,
                                    Crystals = -1
                                };
                            UIService.instance.battleUIHandler.UpdateStats(battle);

                        }, "Error show update RoundUserStatisticsComponent");
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(RoundFundComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    //var battleManager = EntityGroupManagersStorageService.instance.AddOrGetGroupManager<BattleManager, ECSEntity>(entity);
                    ClientInitService.instance.ExecuteInstruction((object Obj) =>
                    {
                        entity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.BattleRoundFund = (component as RoundFundComponent).Fund;
                        UIService.instance.battleUIHandler.UpdateStats(entity);

                    }, null);
                }
            });
        }

        #region killMethods
        public static void KillEntity(KillEvent killEvent)
        {
            if(ManagerScope.entityManager.EntityStorage.TryGetValue(killEvent.WhoDeadId, out var entity) && entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                tankManager.ExecuteInstruction(() => {
                    var groupManager = EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>(entity.instanceId);
                    tankManager.DestroyTank(entity);
                    UIService.instance.battleUIHandler.ShowKillLog(killEvent);
                }, "Error kill");
            }
        }

        #endregion

        

        #region shotAndHit
        public static void Hit(HitEvent hitEvent)
        {
            
        }
        
        public static void StartShooting(StartShootingEvent startShootingEvent)
        {
            if (ManagerScope.entityManager.EntityStorage.TryGetValue(startShootingEvent.EntityOwnerId, out var entity) && entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                tankManager.ExecuteInstruction(() => {
                    tankManager.ProcessedEvent(startShootingEvent);
                }, "Error start shooting");
            }
        }

        public static void StartCharging(StartChargingEvent startChargingEvent)//railgun, shaft, 
        {

        }

        public static void EndShooting(EndShootingEvent endShootingEvent)
        {
            if (ManagerScope.entityManager.EntityStorage.TryGetValue(endShootingEvent.EntityOwnerId, out var entity) && entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                tankManager.ExecuteInstruction(() => {
                    tankManager.ProcessedEvent(endShootingEvent);
                }, "Error start shooting");
            }
        }

        public static void StartAiming(StartAimingEvent startAimingEvent)
        {

        }

        public static void Shot(ShotEvent shotEvent)
        {
            if (ManagerScope.entityManager.EntityStorage.TryGetValue(shotEvent.EntityOwnerId, out var entity) && entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                tankManager.ExecuteInstruction(() => {
                    tankManager.ProcessedEvent(shotEvent);
                }, "Error shot");
            }
        }
        #endregion

        public static void MoveCommand(MoveCommandEvent moveCommandEvent)
        {
            
        }

        public static void SupplyUsed(SupplyUsedEvent supplyUsedEvent)
        {
            
            
        }

        public static void UpdateSupplyCount(UpdateSupplyCountEvent updateSupplyCountEvent)
        {


        }

        #region BattleEvents
        public static void GamePause(GamePauseEvent gamePauseEvent)
        {

        }

        public static void EnterToBattle(EnterToBattleEvent enterToBattleEvent)
        {
            
        }

        public static void ExitFromBatle(LeaveFromBattleEvent leaveFromBattleEvent)
        {
            BattleManager.LoadedBattleClientAction(leaveFromBattleEvent.EntityOwnerId, (battleManagerObj) =>
            {
                var battleManager = battleManagerObj as BattleManager;
                if (battleManager.TryGetValue(leaveFromBattleEvent.EntityOwnerId, out var leaver))
                {
                    leaver.RemoveComponent<BattleOwnerComponent>();
                    //ManagerScope.entityManager.OnRemoveEntity(leaver);
                }
                else
                {
                    ULogger.Log("error leave from battle, may be player exit first");
                }
            });
        }
        #endregion


        #region Drop
        public static void BonusTaking(BonusTakingEvent bonusTakingEvent)
        {
            
        }
        #endregion






        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>()
            {

            }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>()
            {
                IKeys = {
                    KillEvent.Id,
                    HitEvent.Id,
                    ShotEvent.Id,
                    StartShootingEvent.Id,
                    EndShootingEvent.Id,
                    MoveCommandEvent.Id,
                    SupplyUsedEvent.Id,
                    GamePauseEvent.Id,
                    SelfDestructionRequestEvent.Id,
                    EnterToBattleEvent.Id,
                    LeaveFromBattleEvent.Id,
                    BonusTakingEvent.Id
                },
                IValues = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }.Upd();
        }

        public override void Run(long[] entities)
        {
            
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
