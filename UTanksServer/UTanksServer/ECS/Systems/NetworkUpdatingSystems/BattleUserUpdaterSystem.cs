using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.ECSEvents;
using UTanksServer.Extensions;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.ECS.Systems.Battles
{
    public class BattleUserUpdaterSystem : ECSSystem
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
            this.DelayRunMilliseconds = 40;
        }

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { BattlePlayersComponent.Id }, IValues = { 0 } }.Upd();
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
                    try
                    {
                        var entity = ManagerScope.entityManager.EntityStorage[entityId];
                        var battleUsers = entity.GetComponent(BattlePlayersComponent.Id) as BattlePlayersComponent;
                        battleUsers.players.Keys.ForEach((entityPlayers) =>
                        {
                            lock (entityPlayers.contextSwitchLocker)
                            {
                                if (entityPlayers.HasComponent(BattleOwnerComponent.Id) && entityPlayers.HasComponent(UserReadyToBattleComponent.Id))
                                    EntitySerialization.SerializeEntity(entityPlayers, true);
                            }
                        });
                        foreach (var player in battleUsers.players.Keys)
                        {
                            var otherPlayersList = new List<string>();
                            var otherPlayersFastEventsList = new List<List<INetSerializable>>();
                            if (!player.HasComponent(UserReadyToBattleComponent.Id))
                            {
                                continue;
                            }
                            foreach (var otherPlayer in battleUsers.players.Keys)
                            {
                                if (!otherPlayer.HasComponent(UserReadyToBattleComponent.Id))
                                {
                                    continue;
                                }
                                //if(player.instanceId == otherPlayer.instanceId)
                                //{
                                //    continue;
                                //}
                                var otherData = EntitySerialization.BuildSerializedEntityWithGDAP(player, otherPlayer);

                                if (otherData.Item1 != "")
                                    otherPlayersList.Add(otherData.Item1);
                                if (otherData.Item2.Count != 0)
                                    otherPlayersFastEventsList.Add(otherData.Item2);
                            }
                            if (otherPlayersList.Count == 0 && otherPlayersFastEventsList.Count == 0)
                                continue;
                            UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
                            {
                                EntityIdRecipient = player.instanceId,
                                Entities = otherPlayersList
                            };
                            Func<Task> asyncUpd = async () =>
                            {
                                await Task.Run(() => {
                                    var userSocket = (player.GetComponent(UserSocketComponent.Id) as UserSocketComponent);
                                    if (updateEntitiesEvent.Entities.Count > 0)
                                        userSocket.Socket.emit(updateEntitiesEvent.PackToNetworkPacket());
                                    foreach (var rawEventBlock in otherPlayersFastEventsList)
                                    {
                                        foreach (var rawEvent in rawEventBlock)
                                        {
                                            userSocket.Socket.emit(rawEvent);
                                        }
                                    }
                                });
                            };
                            asyncUpd();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("battle user updater error: " + ex.StackTrace + "  " + ex.Message);
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
