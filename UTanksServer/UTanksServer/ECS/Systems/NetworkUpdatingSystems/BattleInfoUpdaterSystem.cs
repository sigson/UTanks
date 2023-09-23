using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Round;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.ECSEvents;
using UTanksServer.Extensions;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.ECS.Systems.NetworkUpdatingSystems
{
    public class BattleInfoUpdaterSystem : ECSSystem
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
            ComponentsOnChangeCallbacks.Add(BattleSimpleInfoComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    var infoUpdater = ManagerScope.systemManager.SystemsInterestedEntityDatabase.Where(x => x.Key.GetType() == typeof(BattleInfoUpdaterSystem)).ToList()[0];
                    infoUpdater.Key.InWork = true;
                    component.DirectiveSetChanged();
                    infoUpdater.Key.Run(infoUpdater.Value.Keys.ToList().ToArray());
                    component.DirectiveSetChanged();
                }
            });
            this.DelayRunMilliseconds = 10000;
            this.Enabled = true;
        }

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { BattleComponent.Id, UserOnlineComponent.Id }, IValues = { 0, 0 } }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { /*Keys = { UserLogged.Id }, Values = { 0 }*/ }.Upd();
        }

        public override void Run(long[] entities)
        {
            List<ECSEntity> freeUsers = new List<ECSEntity>();
            List<ECSEntity> battles = new List<ECSEntity>();
            List<ECSEntity> NewBattles = new List<ECSEntity>();
            foreach (var entityId in entities)
            {
                var entity = ManagerScope.entityManager.EntityStorage[entityId];
                if(entity.HasComponent(UserOnlineComponent.Id) && !entity.HasComponent(BattleOwnerComponent.Id))
                {
                    freeUsers.Add(entity);
                }
                if (entity.HasComponent(BattleComponent.Id))
                {
                    if(entity.entityComponents.ChangedComponent > 0 || entity.GetComponent<BattleSimpleInfoComponent>().MarkedToUpdate)
                    {
                        if (entity.GetComponent<BattlePlayersComponent>().players.Count == 0)
                            EntitySerialization.SerializeEntity(entity, true);
                        //if (entity.entityComponents.CheckChanged(entity.GetComponent<BattleComponent>(BattleComponent.Id).GetTypeFast()))
                        //    NewBattles.Add(entity);
                        if(!entity.HasComponent(RoundRestartingStateComponent.Id))
                        {
                            BattleSimpleInfoUpdater(entity);
                        }
                        else
                        {
                            BattleSimpleInfoUpdater(entity, true, entity.GetComponent<RoundRestartingStateComponent>());
                        }
                        //EntitySerialization.SerializeEntity(entity, true);
                        battles.Add(entity);
                    }
                }
            }
            List<string> battleDataList = new List<string>();
            List<List<INetSerializable>> fastBattleDataList = new List<List<INetSerializable>>();
            foreach (var battle in battles)
            {
                (string, List<INetSerializable>) battleData = ("", new List<INetSerializable>());
                if (NewBattles.Contains(battle))
                {
                    battleData.Item1 = EntitySerialization.BuildFullSerializedEntityWithGDAP(battle, battle);
                }
                else
                {
                    battleData.Item1 = EntitySerialization.BuildFullSerializedEntityWithGDAP(battle, battle);
                }

                if(battleData.Item1 != "")
                    battleDataList.Add(battleData.Item1);
                if(battleData.Item2.Count != 0)
                    fastBattleDataList.Add(battleData.Item2);
            }
            ParallelLoopResult result = Parallel.ForEach<ECSEntity>(
                freeUsers,
                (ECSEntity entity) =>
                {
                    //battleUsers.players.Keys.ForEach((entity) => EntitySerialization.SerializeEntity(entity, true));
                    
                    //if (battleDataList.Count != 0 && fastBattleDataList.Count != 0)
                    {
                        UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
                        {
                            EntityIdRecipient = entity.instanceId,
                            Entities = battleDataList
                        };
                        Func<Task> asyncUpd = async () =>
                        {
                            await Task.Run(() => {
                                var userSocket = (entity.GetComponent(UserSocketComponent.Id) as UserSocketComponent);
                                if(updateEntitiesEvent.Entities.Count > 0)
                                    userSocket.Socket.emit(updateEntitiesEvent.CachePackToNetworkPacket());
                                foreach (var rawEventBlock in fastBattleDataList)
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
            );
            this.LastEndExecutionTimestamp = DateTime.Now.Ticks;
            this.InWork = false;
        }

        public static void BattleSimpleInfoUpdater(ECSEntity battleEntity, bool silent = true, RoundRestartingStateComponent roundRestartingStateComponent = null)
        {
            var battleSimpleInfoComponent = battleEntity.GetComponent<BattleSimpleInfoComponent>(BattleSimpleInfoComponent.Id);
            battleSimpleInfoComponent.MarkedToUpdate = false;
            battleSimpleInfoComponent.locker.EnterWriteLock();
            battleSimpleInfoComponent.battleSimpleInfo.BattleEntityId = battleEntity.instanceId;
            battleSimpleInfoComponent.battleSimpleInfo.BattleRoundFund = battleEntity.GetComponent<RoundFundComponent>(RoundFundComponent.Id).Fund;
            if (roundRestartingStateComponent == null)
                battleSimpleInfoComponent.battleSimpleInfo.TimeRemain = battleEntity.GetComponent<RoundTimerComponent>(RoundTimerComponent.Id).TimeRemaining;
            else
                battleSimpleInfoComponent.battleSimpleInfo.TimeRemain = 0;
            var battleTeamsComp = battleEntity.GetComponent<BattleTeamsComponent>(BattleTeamsComponent.Id);
            foreach (var team in battleTeamsComp.teams)
            {
                if (battleSimpleInfoComponent.battleSimpleInfo.Commands.TryGetValue(team.Value.instanceId, out _))
                {
                    var teamObject = battleSimpleInfoComponent.battleSimpleInfo.Commands[team.Value.instanceId];
                    teamObject.GoalScore = team.Value.GoalScore;
                    teamObject.TeamColor = team.Value.TeamColor;
                }
                else
                {
                    battleSimpleInfoComponent.battleSimpleInfo.Commands.Add(team.Value.instanceId, new Types.Battle.Command() {
                        GoalScore = team.Value.GoalScore,
                        TeamColor = team.Value.TeamColor
                    });
                    
                }
                
            }
            //battleSimpleInfoComponent.battleSimpleInfo. = battleEntity.instanceId;
            var battlePlayers = battleEntity.GetComponent<BattlePlayersComponent>(BattlePlayersComponent.Id).players;

            List<long> checkedObject = new List<long>();

            foreach (var battlePlayer in battlePlayers)
            {
                var teamObject = battleSimpleInfoComponent.battleSimpleInfo.Commands[battlePlayer.Value.instanceId];
                if (teamObject.commandPlayers.TryGetValue(battlePlayer.Key.instanceId, out _))
                {
                    var playerObject = teamObject.commandPlayers[battlePlayer.Key.instanceId];
                    playerObject.Rank = battlePlayer.Key.GetComponent<UserRankComponent>(UserRankComponent.Id).Rank;
                    var userStat = battlePlayer.Key.GetComponent<RoundUserStatisticsComponent>(RoundUserStatisticsComponent.Id);
                    playerObject.Killed = userStat.Kills;
                    playerObject.Death = userStat.Deaths;
                    playerObject.Place = userStat.Place;
                    playerObject.Score = userStat.ScoreWithoutBonuses;
                    playerObject.Username = userStat.Nickname;
                    if (roundRestartingStateComponent == null)
                        playerObject.Crystals = -1;
                    else
                        playerObject.Crystals = (int)roundRestartingStateComponent.rewardStorage[battlePlayer.Key.instanceId];
                    checkedObject.Add(battlePlayer.Key.instanceId);
                }
                else
                {
                    var playerObject = new Types.Battle.CommandPlayers();
                    playerObject.Rank = battlePlayer.Key.GetComponent<UserRankComponent>(UserRankComponent.Id).Rank;
                    var userStat = battlePlayer.Key.TryGetComponent<RoundUserStatisticsComponent>();
                    if (userStat == null)
                        continue;
                    playerObject.Killed = userStat.Kills;
                    playerObject.Death = userStat.Deaths;
                    playerObject.Place = userStat.Place;
                    playerObject.Score = userStat.ScoreWithoutBonuses;
                    playerObject.Username = userStat.Nickname;
                    playerObject.EntityId = battlePlayer.Key.instanceId;
                    if (roundRestartingStateComponent == null)
                        playerObject.Crystals = -1;
                    else
                        playerObject.Crystals = (int)roundRestartingStateComponent.rewardStorage[battlePlayer.Key.instanceId];
                    teamObject.commandPlayers[battlePlayer.Key.instanceId] = playerObject;
                    checkedObject.Add(battlePlayer.Key.instanceId);
                }
            }

            battleSimpleInfoComponent.battleSimpleInfo.Commands.ForEach((command) => command.Value.commandPlayers.ForEach((simpleComPlayer) =>
            {
                if(!checkedObject.Contains(simpleComPlayer.Key))
                {
                    command.Value.commandPlayers.Remove(simpleComPlayer.Key);
                }
            }));
            battleSimpleInfoComponent.locker.ExitWriteLock();
            if (!silent)
                battleSimpleInfoComponent.MarkAsChanged();
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            throw new NotImplementedException();
        }
    }
}
