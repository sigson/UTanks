using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Battle.Tank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using UTanksClient.ECS.Components.ECSComponents;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle;
using UTanksClient.ECS.Templates.Battle;
using UTanksClient.Extensions;
using UTanksClient.Network.NetworkEvents.FastGameEvents;

namespace UTanksClient.ECS.Systems.Battles
{
    public class BattleTankPositionUpdaterSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(CreateBattleEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    //CreateBattle(Event as CreateBattleEvent);
                    return null;
                }
            });
            SystemEventHandler.Add(RemoveBattleEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    //RemoveBattle(Event as RemoveBattleEvent);
                    return null;
                }
            });
            this.Enabled = true;
            this.DelayRunMilliseconds = 200;
        }

        

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>()
            {
                IKeys = {
                    UserEmailComponent.Id//100% local user
                },
                IValues = { 0 }
            }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>()
            {
                IKeys = {
                    CreateBattleEvent.Id
                },
                IValues = { 0 }
            }.Upd();
        }

        public override void Run(long[] entities)
        {
            if(entities.Length > 0)
            {
                if (ManagerScope.entityManager.EntityStorage.TryGetValue(ClientNetworkService.instance.PlayerEntityId, out var playerEntity) &&   playerEntity.HasComponent(BattleOwnerComponent.Id))
                {
                    if (playerEntity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var manager))
                    {
                        manager.ExecuteInstruction(() => {
                            if (manager.hullManager.chassisManager.TankMovable)
                            {
                                var preparedMovement = manager.hullManager.chassisManager.preparedMovementEvent;
                                if (preparedMovement.PlayerEntityId != 0)
                                    ClientNetworkService.instance.Socket.emit(preparedMovement);
                            }
                        });
                    }
                }
                    
                //Func<Task> asyncUpd = async () =>
                //{
                //    await Task.Run(() => {
                //        ClientInit.battleManager.ExecuteInstruction((object Obj) =>
                //        {


                //        }, null);
                //    });
                //};
                //asyncUpd();
            }
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
