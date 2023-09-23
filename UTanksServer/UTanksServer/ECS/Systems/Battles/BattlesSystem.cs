using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Bonus;
using UTanksServer.ECS.Components.Battle.Creature;
using UTanksServer.ECS.Components.Battle.Energy;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.Components.Battle.Location;
using UTanksServer.ECS.Components.Battle.Round;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle;
using UTanksServer.ECS.Events.Battle.BonusEvents;
using UTanksServer.ECS.Events.Battle.TankEvents;
using UTanksServer.ECS.Events.Battle.TankEvents.Shooting;
using UTanksServer.ECS.Events.ECSEvents;
using UTanksServer.ECS.Events.Garage;
using UTanksServer.ECS.Systems.NetworkUpdatingSystems;
using UTanksServer.ECS.Templates.Battle;
using UTanksServer.ECS.Templates.User;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.ECS.Types.Battle.AtomicType;
using UTanksServer.ECS.Types.Battle.Bonus;
using UTanksServer.Extensions;
using UTanksServer.Network.NetworkEvents.FastGameEvents;

namespace UTanksServer.ECS.Systems.Battles
{
    public class BattlesSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            throw new NotImplementedException();
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
            SystemEventHandler.Add(BattleLoadedEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    PlayerReadyToBattle(Event as BattleLoadedEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(HitEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    Hit(Event as HitEvent);
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
            SystemEventHandler.Add(CreatureActuationEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    CreatureActuation(Event as CreatureActuationEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(StartShootingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    StartShooting(Event as StartShootingEvent);
                    return null;
                }
            });
            SystemEventHandler.Add(StartChargingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    StartCharging(Event as StartChargingEvent);
                    return null;
                }
            });
            SystemEventHandler.Add(StartAimingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    StartAiming(Event as StartAimingEvent);
                    return null;
                }
            });
            SystemEventHandler.Add(EndShootingEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    EndShooting(Event as EndShootingEvent);
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
                    return null;
                }
            });
            SystemEventHandler.Add(GamePauseEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    GamePause(Event as GamePauseEvent);
                    return null;
                }
            });
            SystemEventHandler.Add(SelfDestructionRequestEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    SelfDestruction(Event as SelfDestructionRequestEvent);
                    Event.eventWatcher.Watchers--;
                    return null;
                }
            });
            SystemEventHandler.Add(EnterToBattleEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    EnterToBattle(Event as EnterToBattleEvent);
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
                    return null;
                }
            });
        }

        #region killMethods
        public static void KillEntity(KillEvent killEvent)
        {
            var battleEntity = ManagerScope.entityManager.EntityStorage[killEvent.BattleId];
            var killerEntity = ManagerScope.entityManager.EntityStorage[killEvent.WhoKilledId];
            var deadEntity = ManagerScope.entityManager.EntityStorage[killEvent.WhoDeadId];
            var battlePlayers = (battleEntity.GetComponent(BattlePlayersComponent.Id) as BattlePlayersComponent).players.Keys;
            battleEntity.GetComponent<BattleCreatureStorageComponent>().RemoveComponentsByType(new List<long> { MineCreatureComponent.Id }, new List<ECSEntity> { deadEntity });
            battleEntity.GetComponent<BattleCreatureStorageComponent>().MarkAsChanged();
            deadEntity.AddComponent(new TankDeadStateComponent()
            {
                onEnd = (delegateEntity, delegateComponent) =>
                {
                    delegateEntity.TryRemoveComponent(delegateComponent.GetId());//error
                },
                //timerAwait = 3000
            }.TimerStart(3, deadEntity, true).SetGlobalComponentGroup());
            deadEntity.GetComponent<CharacteristicTransformersComponent>().characteristicTransformers.Values.ForEach((x) => {
                if (!x.stableTransformer)
                    deadEntity.TryRemoveComponent((x as ECSComponent).GetId());
            });
            deadEntity.GetComponent<TankCooldownStorageComponent>().CooldownStorage.Clear();

            foreach (var player in battlePlayers)
            {
                var socket = (UserSocketComponent)player.GetComponent(UserSocketComponent.Id);
                socket.Socket.emit<GameDataEvent>(killEvent.PackToNetworkPacket());
            }
            var deadEntityTeam = deadEntity.GetComponent(TeamComponent.Id) as TeamComponent;
            
            RespawnTank(battleEntity, killerEntity, deadEntity, deadEntityTeam);

            var deadStat = deadEntity.GetComponent(RoundUserStatisticsComponent.Id) as RoundUserStatisticsComponent;
            deadStat.Deaths++;
            deadStat.MarkAsChanged();
            if (killEvent.WhoKilledId != killEvent.WhoDeadId)
            {
                FundUpdate(killEvent);

                UserTemplate.ScoreUpdate(killerEntity, 10);
                killerEntity.GetComponent<UserScoreComponent>().MarkAsChanged();

                var killerEntityTeam = killerEntity.GetComponent(TeamComponent.Id) as TeamComponent;

                var killerStat = killerEntity.GetComponent(RoundUserStatisticsComponent.Id) as RoundUserStatisticsComponent;
                killerStat.Kills++;
                killerStat.ScoreWithoutBonuses += (int)GlobalGameDataConfig.SelectableMap.selectableMaps.KillUncoditionalInBattleScorePoints;
                killerStat.MarkAsChanged();
                if (battleEntity.HasComponent(DMComponent.Id))
                {
                    if(killerEntityTeam.GoalScore < killerStat.Kills)
                        killerEntityTeam.GoalScore++;
                }
                if (battleEntity.HasComponent(TDMComponent.Id))
                {
                    killerEntityTeam.GoalScore++;
                }
            }
            battleEntity.GetComponent<BattleScoreComponent>().MarkAsChanged();
            battleEntity.GetComponent<BattleSimpleInfoComponent>().MarkedToUpdate = true;
        }

        public static void FundUpdate(ECSEvent fundEvent)
        {
            if (fundEvent.GetId() == KillEvent.Id)
            {
                var fundEventType = fundEvent as KillEvent;
                var battle = ManagerScope.entityManager.EntityStorage[fundEventType.BattleId];
                var dead = ManagerScope.entityManager.EntityStorage[fundEventType.WhoDeadId];
                var battleFund = battle.GetComponent(RoundFundComponent.Id) as RoundFundComponent;
                battleFund.UpdateFundActions();
                var deadRank = (dead.GetComponent(UserRankComponent.Id) as UserRankComponent).Rank;
                battleFund.Fund = (battleFund.FundCoef * GlobalGameDataConfig.SelectableMap.selectableMaps.KillUnconditionalFundReward) + (battleFund.FundCoef * GlobalGameDataConfig.SelectableMap.selectableMaps.KillRankKoefReward * deadRank);
                battleFund.MarkAsChanged();
            }
            
        }

        public static void SelfDestruction(SelfDestructionRequestEvent selfDestructionRequestEvent)
        {
            var ownerEntity = ManagerScope.entityManager.EntityStorage[selfDestructionRequestEvent.EntityOwnerId];
            if(!ownerEntity.HasComponent(TankSpawnStateComponent.Id) && !ownerEntity.HasComponent(TankDeadStateComponent.Id))
                ownerEntity.TryAddComponent(new SelfDestructionComponent(5f).SetGlobalComponentGroup());
        }
        #endregion

        public static void RespawnTank(ECSEntity battleEntity, ECSEntity killerEntity, ECSEntity deadEntity, TeamComponent teamComponent, bool firstSpawn = false)
        {
            var mapComponent = battleEntity.GetComponent(MapComponent.Id) as MapComponent;
            if(firstSpawn)
            {
                var worldPos = deadEntity.GetComponent<WorldPositionComponent>();
                worldPos.WorldPoint = mapComponent.map.SpawnPoints[teamComponent.TeamColor][new Random().Next(0, mapComponent.map.SpawnPoints[teamComponent.TeamColor].Count - 1)];
                deadEntity.AddComponent(new TankNewStateComponent().SetGlobalComponentGroup());
                var spawn = deadEntity.GetComponent<TankNewStateComponent>();
                //spawn.MarkAsChanged();
                worldPos.MarkAsChanged();
                spawn.MarkAsChanged();
            }
            else
            {
                deadEntity.AddComponent(new TankSpawnStateComponent()
                {
                    spawnPosition = mapComponent.map.SpawnPoints[teamComponent.TeamColor][new Random().Next(0, mapComponent.map.SpawnPoints[teamComponent.TeamColor].Count - 1)]
                }.SetGlobalComponentGroup());
                var spawn = deadEntity.GetComponent<TankSpawnStateComponent>();
                var worldPos = deadEntity.GetComponent<WorldPositionComponent>();
                worldPos.WorldPoint = spawn.spawnPosition;
                //spawn.MarkAsChanged();
                worldPos.MarkAsChanged();
                spawn.MarkAsChanged();
            }
            
        }

        #region shotAndHit
        public static void Hit(HitEvent hitEvent)
        {
            var HitPlayer = ManagerScope.entityManager.EntityStorage[hitEvent.EntityOwnerId];
            //ParallelLoopResult result = Parallel.ForEach<KeyValuePair<long, Vector3S>>(
            //    hitEvent.hitList,
            //    (hitKeyPair) =>
            //    {

            //    }
            //);

            var energyUsingData = HitPlayer.TryGetComponent<StreamWeaponEnergyComponent>();
            if(energyUsingData != null)
            {
                var energyData = HitPlayer.GetComponent<WeaponEnergyComponent>(WeaponEnergyComponent.Id);

                if (!(energyData.Energy - energyUsingData.UnloadEnergyPerSec <= 0))
                {
                    foreach (var hitKeyPair in hitEvent.hitList)
                    {
                        Func<Task> asyncUpd = async () =>
                        {
                            await Task.Run(() => {
                                var hittingEntity = ManagerScope.entityManager.EntityStorage[hitKeyPair.Key];
                                var distance = hitEvent.hitDistanceList[hitKeyPair.Key];
                                var effectsAggreg = HitPlayer.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent;
                                var randomSalt = (float)new Random().NextDouble();
                                var hittingEntityHealth = hittingEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                                var oldHealth = hittingEntityHealth.CurrentHealth;
                                foreach (var effect in effectsAggreg.effectsAggregator.Values)
                                {
                                    effect(HitPlayer, hittingEntity, randomSalt, hitEvent);
                                }
                                var newHealth = oldHealth - hittingEntityHealth.CurrentHealth;
                                if (oldHealth != newHealth)
                                {
                                    (HitPlayer.GetComponent(UserSocketComponent.Id) as UserSocketComponent).Socket.emit(new ShotHPResultEvent() { Damage = newHealth, StruckEntityId = hitKeyPair.Key }.PackToNetworkPacket());
                                }

                            });
                        };
                        asyncUpd();
                    }
                }
            }
            else
            {
                var energyData = HitPlayer.GetComponent<WeaponEnergyComponent>(WeaponEnergyComponent.Id);
                foreach (var hitKeyPair in hitEvent.hitList)
                {
                    Func<Task> asyncUpd = async () =>
                    {
                        await Task.Run(() => {
                            var hittingEntity = ManagerScope.entityManager.EntityStorage[hitKeyPair.Key];
                            var distance = hitEvent.hitDistanceList[hitKeyPair.Key];
                            var effectsAggreg = HitPlayer.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent;
                            var randomSalt = (float)new Random().NextDouble();
                            var hittingEntityHealth = hittingEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                            var oldHealth = hittingEntityHealth.CurrentHealth;
                            foreach (var effect in effectsAggreg.effectsAggregator.Values)
                            {
                                effect(HitPlayer, hittingEntity, randomSalt, hitEvent);
                            }
                            var newHealth = oldHealth - hittingEntityHealth.CurrentHealth;
                            if (oldHealth != newHealth)
                            {
                                (HitPlayer.GetComponent(UserSocketComponent.Id) as UserSocketComponent).Socket.emit(new ShotHPResultEvent() { Damage = newHealth, StruckEntityId = hitKeyPair.Key }.PackToNetworkPacket());
                            }

                        });
                    };
                    asyncUpd();
                }
            }
            
        }
        
        public static void StartShooting(StartShootingEvent startShootingEvent)
        {
            var StartShootPlayer = ManagerScope.entityManager.EntityStorage[startShootingEvent.EntityOwnerId];
            var battleOwner = StartShootPlayer.GetComponent<BattleOwnerComponent>(BattleOwnerComponent.Id);
            foreach (var player in battleOwner.Battle.GetComponent<BattlePlayersComponent>(BattlePlayersComponent.Id).players.Keys)
            {
                if (player.instanceId == StartShootPlayer.instanceId)
                    continue;
                player.GetComponent<UserSocketComponent>(UserSocketComponent.Id).Socket.emit((RawStartShootingEvent)startShootingEvent.cachedRawEvent);
            }
            if(StartShootPlayer.HasComponent(StreamWeaponEnergyComponent.Id))
            {
                StartShootPlayer.TryAddComponent(new WeaponStreamShootingComponent(StartShootPlayer.GetComponent<StreamWeaponEnergyComponent>(StreamWeaponEnergyComponent.Id)));
            }
            if (StartShootPlayer.HasComponent(DiscreteWeaponEnergyComponent.Id))
            {
                if(StartShootPlayer.HasComponent(RailgunDamageComponent.Id))
                {
                    StartShootPlayer.TryAddComponent(new RailgunChargingWeaponComponent(StartShootPlayer.GetComponent<RailgunDamageComponent>().chargeTimeProperty));
                }
                
            }

        }

        public static void StartCharging(StartChargingEvent startChargingEvent)//railgun, shaft, 
        {

        }

        public static void EndShooting(EndShootingEvent endShootingEvent)
        {
            var EndShootingEntity = ManagerScope.entityManager.EntityStorage[endShootingEvent.EntityOwnerId];
            var battleOwner = EndShootingEntity.GetComponent<BattleOwnerComponent>(BattleOwnerComponent.Id);
            foreach (var player in battleOwner.Battle.GetComponent<BattlePlayersComponent>(BattlePlayersComponent.Id).players.Keys)
            {
                if (player.instanceId == EndShootingEntity.instanceId)
                    continue;
                player.GetComponent<UserSocketComponent>(UserSocketComponent.Id).Socket.emit((RawEndShootingEvent)endShootingEvent.cachedRawEvent);
            }
            EndShootingEntity.TryRemoveComponent(WeaponStreamShootingComponent.Id);
        }

        public static void StartAiming(StartAimingEvent startAimingEvent)
        {

        }

        public static void Shot(ShotEvent shotEvent)
        {
            var ShootPlayer = ManagerScope.entityManager.EntityStorage[shotEvent.EntityOwnerId];
            //ParallelLoopResult result = Parallel.ForEach<KeyValuePair<long, Vector3S>>(
            //    hitEvent.hitList,
            //    (hitKeyPair) =>
            //    {

            //    }
            //);

            var energyUsingData = ShootPlayer.GetComponent<DiscreteWeaponEnergyComponent>(DiscreteWeaponEnergyComponent.Id);
            var energyData = ShootPlayer.GetComponent<WeaponEnergyComponent>(WeaponEnergyComponent.Id);
            if (energyData.Energy - energyUsingData.UnloadEnergyPerShot >= 0)
            {
                energyData.Energy -= energyUsingData.UnloadEnergyPerShot;
                energyData.MarkAsChanged();

                //if (energyData.Energy == 0)
                //    ShootPlayer.TryAddComponent(new WeaponCooldownComponent(2f).SetGlobalComponentGroup());

                foreach (var shotKeyPair in shotEvent.hitList)
                {
                    Func<Task> asyncUpd = async () =>
                    {
                        await Task.Run(() => {
                            ECSEntity hittingEntity = null;
                            if (shotKeyPair.Key != -1)
                            {
                                hittingEntity = ManagerScope.entityManager.EntityStorage[shotKeyPair.Key];
                            }

                            var distance = shotEvent.hitDistanceList[shotKeyPair.Key];

                            if (hittingEntity != null && (!hittingEntity.HasComponent(TankDeadStateComponent.Id) && !hittingEntity.HasComponent(TankNewStateComponent.Id) && !hittingEntity.HasComponent(TankSpawnStateComponent.Id)))
                            {
                                var effectsAggreg = ShootPlayer.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent;
                                var randomSalt = (float)new Random().NextDouble();
                                var hittingEntityHealth = hittingEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                                var oldHealth = hittingEntityHealth.CurrentHealth;
                                foreach (var effect in effectsAggreg.effectsAggregator.Values)
                                {
                                    effect(ShootPlayer, hittingEntity, randomSalt, shotEvent);
                                }
                                var newHealth = oldHealth - hittingEntityHealth.CurrentHealth;
                                (ShootPlayer.GetComponent(UserSocketComponent.Id) as UserSocketComponent).Socket.emit(new ShotHPResultEvent() { Damage = newHealth, StruckEntityId = shotKeyPair.Key }.PackToNetworkPacket());
                            }
                        });
                    };
                    asyncUpd();
                    
                }
                var battle = ManagerScope.entityManager.EntityStorage[ShootPlayer.GetComponent<BattleOwnerComponent>().BattleInstanceId];
                var players = battle.GetComponent<BattlePlayersComponent>().players.Keys.ToList();
                foreach (var player in players)
                {
                    if (player.instanceId == ShootPlayer.instanceId)
                        continue;
                    player.GetComponent<UserSocketComponent>().Socket.emit<RawShotEvent>((RawShotEvent)shotEvent.cachedRawEvent);
                }
            }
            
        }

        public static void CreatureActuation(CreatureActuationEvent creatureActuationEvent)
        {
            ECSEntity CreatureDBBattle = null;
            if(ManagerScope.entityManager.EntityStorage.TryGetValue(creatureActuationEvent.BattleDBOwnerId, out CreatureDBBattle))
            {
                var creatureDB = CreatureDBBattle.GetComponent<BattleCreatureStorageComponent>();
                var creature = creatureDB.GetComponent(creatureActuationEvent.CreatureInstanceId, ManagerScope.entityManager.EntityStorage[creatureActuationEvent.EntityOwnerId]);
                if(creature.Item1 == null)
                {
                    Logger.LogError("Error get creature");
                    return;
                }
                List<ECSEntity> targets = new List<ECSEntity>();
                creatureActuationEvent.TargetsId.ForEach(x => targets.Add(ManagerScope.entityManager.EntityStorage[x]));
                (creature.Item1 as ICreatureComponent).OnCreatureActuation(targets, creatureDB);
            }
        }
        #endregion

        public static void MoveCommand(MoveCommandEvent moveCommandEvent)
        {
            var MovementPlayer = ManagerScope.entityManager.EntityStorage[moveCommandEvent.EntityOwnerId];
            BattleOwnerComponent battleOwner = null;
            lock(MovementPlayer.entityComponents.operationLocker)
            {
                if (!MovementPlayer.HasComponent(BattleOwnerComponent.Id) || MovementPlayer.HasComponent(TankDeadStateComponent.Id) || MovementPlayer.HasComponent(TankSpawnStateComponent.Id))
                    return;
                battleOwner = MovementPlayer.GetComponent<BattleOwnerComponent>(BattleOwnerComponent.Id);
                MovementPlayer.GetComponent<WorldPositionComponent>(WorldPositionComponent.Id).WorldPoint = new WorldPoint { Position = moveCommandEvent.position, Rotation = MathEx.ToEulerAngles(new System.Numerics.Quaternion(moveCommandEvent.rotation.x, moveCommandEvent.rotation.y, moveCommandEvent.rotation.z, moveCommandEvent.rotation.w)) };
                //MovementPlayer.GetComponent<WorldPositionComponent>().MarkAsChanged();
                
            }
            foreach (var player in battleOwner.Battle.GetComponent<BattlePlayersComponent>(BattlePlayersComponent.Id).players.Keys)
            {
                if (player.instanceId != MovementPlayer.instanceId)
                    player.GetComponent<UserSocketComponent>(UserSocketComponent.Id).Socket.emit((RawMovementEvent)moveCommandEvent.cachedRawEvent);
            }

            //moveCommandEvent.eventWatcher.Watchers--;
        }

        public static void SupplyUsed(SupplyUsedEvent supplyUsedEvent)
        {
            ECSEntity playerEntity = null;
            if (ManagerScope.entityManager.EntityStorage.TryGetValue(supplyUsedEvent.EntityOwnerId, out playerEntity))
            {
                var cooldownStorage = playerEntity.GetComponent<TankCooldownStorageComponent>();
                long supCooldown = 0;
                if(cooldownStorage.CooldownStorage.TryGetValue(supplyUsedEvent.supplyPath, out supCooldown))
                {
                    if (DateTime.Now.Ticks < supCooldown)
                        return;
                    //ddos
                }
                var battleEquipment = playerEntity.GetComponent<UserBattleGarageDBComponent>().battleEquipment;
                var findedSupply = battleEquipment.Supplies.Where(x => x.PathName == supplyUsedEvent.supplyPath).ToList();
                if(findedSupply.Count > 0)
                {
                    if(findedSupply[0].Count > 0)
                    {
                        findedSupply[0].Count--;
                        
                        SupplyTemplate.UseSupply(playerEntity, supplyUsedEvent, findedSupply[0].PathName);
                        UpdateSupplyCountEvent updateSupplyCountEvent = new UpdateSupplyCountEvent() { EntityOwnerId = playerEntity.instanceId };
                        BattleUserTemplate.GenerateBattleUserEquipment(playerEntity, playerEntity.GetComponent<BattleOwnerComponent>().Battle, updateSupplyCountEvent);
                        if (updateSupplyCountEvent.InBattleSupply.Count > 0 || updateSupplyCountEvent.SyncSupply.Count > 0)
                        {
                            playerEntity.GetComponent<UserSocketComponent>().Socket.emit(updateSupplyCountEvent.PackToNetworkPacket());
                        }
                        if (playerEntity.GetComponent<BattleOwnerComponent>().Battle.GetComponent<BattleComponent>().enablePlayerSupplies)
                        {
                            var realPlayerSupply = playerEntity.GetComponent<UserGarageDBComponent>().garage.Supplies.Where(x => x.PathName == supplyUsedEvent.supplyPath).ToList();
                            if (realPlayerSupply.Count > 0)
                            {
                                realPlayerSupply[0].Count = findedSupply[0].Count;
                            }
                            if (realPlayerSupply[0].Count == 0)
                            {
                                playerEntity.GetComponent<UserGarageDBComponent>().garage.Supplies.Remove(realPlayerSupply[0]);
                                playerEntity.GetComponent<UserGarageDBComponent>().MarkAsChanged();
                            }
                        }
                    }
                }
            }
        }

        #region BattleEvents

        public void PlayerReadyToBattle(BattleLoadedEvent battleLoadedEvent)
        {
            var battleEntity = ManagerScope.entityManager.EntityStorage[battleLoadedEvent.BattleId];
            var player = ManagerScope.entityManager.EntityStorage[battleLoadedEvent.EntityOwnerId];
            var teamComp = player.TryGetComponent<TeamComponent>();
            while(teamComp == null)
            {
                Thread.Sleep(50);
                teamComp = player.TryGetComponent<TeamComponent>();
            }

            BattleInfoUpdaterSystem.BattleSimpleInfoUpdater(battleEntity);
            var battleDataList = new List<string>() { EntitySerialization.BuildFullSerializedEntityWithGDAP(battleEntity, battleEntity) };
            UpdateEntitiesEvent updateBattleEvent = new UpdateEntitiesEvent()
            {
                EntityIdRecipient = player.instanceId,
                Entities = battleDataList
            };
            Func<Task> asyncBattleUpd = async () =>
            {
                await Task.Run(() => {
                    var userSocket = (player.GetComponent(UserSocketComponent.Id) as UserSocketComponent);
                    if (updateBattleEvent.Entities.Count > 0)
                        userSocket.Socket.emit(updateBattleEvent.CachePackToNetworkPacket());
                });
            };
            asyncBattleUpd();


            RespawnTank(battleEntity, null, player, teamComp, true);


            var battleUsers = battleEntity.GetComponent(BattlePlayersComponent.Id) as BattlePlayersComponent;
            //battleUsers.players.Keys.ForEach((entity) => EntitySerialization.SerializeEntity(entity, true));
            //EntitySerialization.SerializeEntity(player);

            foreach (var otherPlayer in battleUsers.players.Keys)
            {
                //if (player.instanceId == otherPlayer.instanceId)
                //{
                //    continue;
                //}
                var otherData = EntitySerialization.BuildFullSerializedEntityWithGDAP(otherPlayer, player);
                if (otherData == "")
                    continue;
                UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
                {
                    EntityIdRecipient = otherPlayer.instanceId,
                    Entities = new List<string> { otherData }
                };

                Func<Task> asyncUpd = async () =>
                {
                    await Task.Run(() => {
                        otherPlayer.GetComponent<UserSocketComponent>(UserSocketComponent.Id).Socket.emit(updateEntitiesEvent.CachePackToNetworkPacket());
                    });
                };
                asyncUpd();
                
            }
            player.AddComponent(new UserReadyToBattleComponent().SetGlobalComponentGroup());
            //battleEntity.GetComponent<BattleSimpleInfoComponent>().MarkAsChanged();

            #region mineUpdate
            var battleSerialized = EntitySerialization.BuildFullSerializedEntityWithGDAP(player, battleEntity);
            var userSocket = player.GetComponent(UserSocketComponent.Id) as UserSocketComponent;
            UpdateEntitiesEvent updateBattleEntityEvent = new UpdateEntitiesEvent()
            {
                EntityIdRecipient = battleLoadedEvent.EntityOwnerId,
                Entities = new List<string>() { battleSerialized }
            };

            Func<Task> asyncUpd2 = async () =>
            {
                await Task.Run(() => {
                    userSocket.Socket.emit(updateBattleEntityEvent.CachePackToNetworkPacket());
                });
            };
            asyncUpd2();
            #endregion
            //if (entitySerialized == "")
            //    return;
            //var userSocket = player.GetComponent(UserSocketComponent.Id) as UserSocketComponent;
            //UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
            //{
            //    EntityIdRecipient = player.instanceId,
            //    Entities = new List<string>()
            //            {
            //                entitySerialized
            //            }
            //};

            //Func<Task> asyncUpd = async () =>
            //{
            //    await Task.Run(() => {
            //        userSocket.Socket.emit(updateEntitiesEvent.PackToNetworkPacket());
            //    });
            //};
            //asyncUpd();
        }

        public static void GamePause(GamePauseEvent gamePauseEvent)
        {

        }

        public static void EnterToBattle(EnterToBattleEvent enterToBattleEvent)
        {
            var entity = ManagerScope.entityManager.EntityStorage[enterToBattleEvent.BattleId];
            while(entity.HasComponent<RoundRestartingStateComponent>())
            {
                Task.Delay(10).Wait();
            }
            var newPlayer = ManagerScope.entityManager.EntityStorage[enterToBattleEvent.EntityOwnerId];
            if(newPlayer.HasComponent<BattleOwnerComponent>())
            {
                return;
            }
            new BattleUserTemplate().EnterToBattleUpdateEntity(newPlayer, ManagerScope.entityManager.EntityStorage[enterToBattleEvent.BattleId], enterToBattleEvent.TeamInstanceId);

            
            entity.GetComponent<BattleSimpleInfoComponent>().MarkAsChanged(true);

            EntitySerialization.SerializeEntity(newPlayer);
            
            var entitySerialized = EntitySerialization.BuildFullSerializedEntityWithGDAP(newPlayer, newPlayer);
            var battleSerialized = EntitySerialization.BuildFullSerializedEntityWithGDAP(newPlayer, entity);

            if (entitySerialized == "")
                return;
            var userSocket = newPlayer.GetComponent(UserSocketComponent.Id) as UserSocketComponent;
            

            
            var battleUsers = entity.GetComponent(BattlePlayersComponent.Id) as BattlePlayersComponent;
            //battleUsers.players.Keys.ForEach((entity) => EntitySerialization.SerializeEntity(entity, true));

            var playerData = new List<string>();
            playerData.Add(entitySerialized);
            
            foreach (var player in battleUsers.players.Keys)
            {
                if (player.instanceId == newPlayer.instanceId)
                {
                    continue;
                }
                var otherData = EntitySerialization.BuildFullSerializedEntityWithGDAP(newPlayer, player);
                if (otherData == "")
                    continue;
                playerData.Add(otherData);
            }
            playerData.Add(battleSerialized);
            UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
            {
                EntityIdRecipient = enterToBattleEvent.EntityOwnerId,
                Entities = playerData
            };

            Func<Task> asyncUpd = async () =>
            {
                await Task.Run(() => {
                    userSocket.Socket.emit(updateEntitiesEvent.CachePackToNetworkPacket());
                });
            };
            asyncUpd();

        }

        public static void ExitFromBatle(LeaveFromBattleEvent leaveFromBattleEvent)
        {
            var battleEntity = ManagerScope.entityManager.EntityStorage[leaveFromBattleEvent.BattleId];
            while (battleEntity.HasComponent<RoundRestartingStateComponent>())
            {
                Task.Delay(10).Wait();
            }
            var playerEntity = ManagerScope.entityManager.EntityStorage[leaveFromBattleEvent.EntityOwnerId];
            if (!playerEntity.HasComponent<BattleOwnerComponent>())
            {
                return;
                //ddosEvent
            }
            else if(playerEntity.GetComponent<BattleOwnerComponent>().BattleInstanceId != leaveFromBattleEvent.BattleId)
            {
                return;
                //ddosEvent
            }
            new BattleUserTemplate().ExitFromBatle(playerEntity, battleEntity, leaveFromBattleEvent.TeamInstanceId, leaveFromBattleEvent);
            battleEntity.GetComponent<BattleCreatureStorageComponent>().RemoveComponentsByType(new List<long> { MineCreatureComponent.Id }, new List<ECSEntity> { playerEntity });
            battleEntity.GetComponent<BattleCreatureStorageComponent>().MarkAsChanged();
            battleEntity.GetComponent<BattleSimpleInfoComponent>().MarkAsChanged(true);
        }
        #endregion


        #region Drop
        public static void BonusTaking(BonusTakingEvent bonusTakingEvent)
        {
            var bonusTakingPlayer = ManagerScope.entityManager.EntityStorage[bonusTakingEvent.EntityOwnerId];
            if (bonusTakingPlayer == null || (bonusTakingPlayer.HasComponent(TankDeadStateComponent.Id) || bonusTakingPlayer.HasComponent(TankNewStateComponent.Id) || bonusTakingPlayer.HasComponent(TankSpawnStateComponent.Id)))
                return;
            var battleOwner = bonusTakingPlayer.GetComponent<BattleOwnerComponent>(BattleOwnerComponent.Id);
            
            BonusComponent DropEntity = null;
            
            if (battleOwner != null && ManagerScope.entityManager.EntityStorage.TryGetValue(battleOwner.BattleInstanceId, out var battleEntity))
            {
                //DropEntity.locker.EnterWriteLock();
                DropEntity = battleEntity.GetComponent<BattleDropStorageComponent>().GetComponent(bonusTakingEvent.DropId, battleEntity).Item1 as BonusComponent;
                var bonusComponent = DropEntity;
                var bonusStateComponent = DropEntity;
                bool updated = false;
                lock(DropEntity.locker)
                {
                    if (bonusStateComponent.bonusState != BonusState.Taken)
                    {
                        bonusStateComponent.bonusState = BonusState.Taken;
                        var battleDropStorage = battleEntity.GetComponent<BattleDropStorageComponent>();
                        battleDropStorage.RemoveComponent(DropEntity.instanceId, DropEntity.ownerEntity);
                        battleDropStorage.MarkAsChanged();
                        updated = true;
                        //bonusStateComponent.MarkAsChanged();
                        
                    }
                }
                if(updated)
                {
                    foreach (var player in battleOwner.Battle.GetComponent<BattlePlayersComponent>(BattlePlayersComponent.Id).players.Keys)
                    {
                        player.GetComponent<UserSocketComponent>(UserSocketComponent.Id).Socket.emit((RawDropTakingEvent)bonusTakingEvent.cachedRawEvent);
                    }
                    UserCrystalsComponent userCrystal = null;
                    switch (bonusComponent.bonusType)
                    {
                        case BonusType.Armor:
                            DropEntity.CharacteristicTransformerContainer.characteristicTransformerComponent.ForEach((x) => {
                                if (!bonusTakingPlayer.TryAddComponent((x as ECSComponent).SetGlobalComponentGroup()))
                                {
                                    var transformer = bonusTakingPlayer.GetComponent<TimerComponent>((x as ECSComponent).GetId());
                                    transformer.TimerReset();
                                    transformer.MarkAsChanged();
                                }
                            });
                            break;
                        case BonusType.Damage:
                            DropEntity.CharacteristicTransformerContainer.characteristicTransformerComponent.ForEach((x) => {
                                if (!bonusTakingPlayer.TryAddComponent((x as ECSComponent).SetGlobalComponentGroup()))
                                {
                                    var transformer = bonusTakingPlayer.GetComponent<TimerComponent>((x as ECSComponent).GetId());
                                    transformer.TimerReset();
                                    transformer.MarkAsChanged();
                                }
                            });
                            break;
                        case BonusType.Speed:
                            DropEntity.CharacteristicTransformerContainer.characteristicTransformerComponent.ForEach((x) => {
                                if (!bonusTakingPlayer.TryAddComponent((x as ECSComponent).SetGlobalComponentGroup()))
                                {
                                    var transformer = bonusTakingPlayer.GetComponent<TimerComponent>((x as ECSComponent).GetId());
                                    transformer.TimerReset();
                                    transformer.MarkAsChanged();
                                }
                            });
                            break;
                        case BonusType.Repair:
                            DropEntity.CharacteristicTransformerContainer.characteristicTransformerComponent.ForEach((x) => {
                                if (!bonusTakingPlayer.TryAddComponent((x as ECSComponent).SetGlobalComponentGroup()))
                                {
                                    var transformer = bonusTakingPlayer.GetComponent<TimerComponent>((x as ECSComponent).GetId());
                                    transformer.TimerReset();
                                    transformer.MarkAsChanged();
                                }
                            });
                            break;
                        case BonusType.Crystal:
                            if (true)
                            {
                                userCrystal = bonusTakingPlayer.GetComponent<UserCrystalsComponent>(UserCrystalsComponent.Id);
                                userCrystal.UserCrystals += (int)GlobalGameDataConfig.SelectableMap.selectableMaps.CrystalDropBonusReward;
                            }
                            break;
                        case BonusType.Container:
                            break;
                        case BonusType.Gold:
                            if (true)
                            {
                                userCrystal = bonusTakingPlayer.GetComponent<UserCrystalsComponent>(UserCrystalsComponent.Id);
                                userCrystal.UserCrystals += (int)GlobalGameDataConfig.SelectableMap.selectableMaps.GoldDropBonusReward;
                            }
                            break;
                        case BonusType.Ruby:
                            if (true)
                            {
                                userCrystal = bonusTakingPlayer.GetComponent<UserCrystalsComponent>(UserCrystalsComponent.Id);
                                userCrystal.UserCrystals += (int)GlobalGameDataConfig.SelectableMap.selectableMaps.RubyDropBonusReward;
                            }
                            break;
                        case BonusType.SuperContainer:
                            break;
                        case BonusType.TestBox:
                            DropEntity.CharacteristicTransformerContainer.characteristicTransformerComponent.ForEach((x) => bonusTakingPlayer.AddComponent(x as ECSComponent));
                            break;
                    }
                    if (userCrystal != null)
                    {
                        userCrystal.MarkAsChanged();
                    }
                }
            }
        }
        #endregion






        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            throw new NotImplementedException();
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
                    BonusTakingEvent.Id,
                    BattleLoadedEvent.Id,
                    CreatureActuationEvent.Id
                },
                IValues = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }.Upd();
        }

        public override void Run(long[] entities)
        {
            throw new NotImplementedException();
        }

        public override void UpdateEventWatcher(ECSEvent eCSEvent)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            throw new NotImplementedException();
        }
    }
}
