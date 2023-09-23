using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Bonus;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.ECSEvents;
using UTanksServer.Extensions;

namespace UTanksServer.ECS.Systems.NetworkUpdatingSystems
{
    public class DropUpdatingSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            //SystemEventHandler.Add(UserLogged.Id, new List<Func<ECSEvent, object>>() {
            //    (Event) => {
            //        AppendUserToECS((UserLogged)Event);
            //        return null;
            //    }
            //});
            //ComponentsOnChangeCallbacks.Add()
            this.Enabled = true;
            this.DelayRunMilliseconds = 250;
        }

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { BonusComponent.Id }, IValues = { 0 } }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { /*Keys = { UserLogged.Id }, Values = { 0 }*/ }.Upd();
        }

        public override void Run(long[] entities)
        {
            ParallelLoopResult result = Parallel.ForEach<long>(
                entities,
                (long entityId) =>
                {
                    ECSEntity dropEntity = null;
                    if (!ManagerScope.entityManager.EntityStorage.TryGetValue(entityId, out dropEntity))
                        return;
                    EntitySerialization.SerializeEntity(dropEntity, true);
                    var serializedDrop = EntitySerialization.BuildSerializedEntityWithGDAP(dropEntity, dropEntity);
                    if (serializedDrop.Item1 == "" && serializedDrop.Item2.Count == 0)
                        return;
                    //var battleEntity = ManagerScope.entityManager.EntityStorage[(dropEntity.GetComponent(BattleOwnerComponent.Id) as BattleOwnerComponent).BattleInstanceId];
                    var battleEntity = (dropEntity.GetComponent(BattleOwnerComponent.Id) as BattleOwnerComponent).Battle;
                    var battleUsers = battleEntity.GetComponent(BattlePlayersComponent.Id) as BattlePlayersComponent;
                    foreach (var player in battleUsers.players.Keys)
                    {
                        UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
                        {
                            EntityIdRecipient = player.instanceId,
                            Entities = new List<string>()
                                {
                                    serializedDrop.Item1
                                }
                        };
                        Func<Task> asyncUpd = async () =>
                        {
                            await Task.Run(() =>
                            {
                                var userSocket = (player.GetComponent(UserSocketComponent.Id) as UserSocketComponent);
                                if(serializedDrop.Item1 != "")
                                    userSocket.Socket.emit(updateEntitiesEvent.PackToNetworkPacket());
                                foreach (var rawEvent in serializedDrop.Item2)
                                {
                                    userSocket.Socket.emit(rawEvent);
                                }
                            });
                        };
                        asyncUpd();
                    }
                }
            );
            this.LastEndExecutionTimestamp = DateTime.Now.Ticks;
            this.InWork = false;
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            throw new NotImplementedException();
        }
    }
}
