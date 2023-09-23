using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Creature;
using UTanksServer.ECS.Components.Battle.Location;
using UTanksServer.ECS.Components.Battle.Round;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.Components.Garage;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.DataAccessPolicy.Battles;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle;
using UTanksServer.ECS.Events.ECSEvents;
using UTanksServer.ECS.Events.Garage;
using UTanksServer.ECS.Systems.NetworkUpdatingSystems;

namespace UTanksServer.ECS.Templates.Battle
{
    [TypeUid(183156117890446200)]
    public class BattleUserTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 183156117890446200;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public ECSEntity EnterToBattleUpdateEntity(ECSEntity userEntity, ECSEntity battleEntity, long teamId)
        {
            var teamComp = (battleEntity.GetComponent(BattleTeamsComponent.Id) as BattleTeamsComponent).teams[teamId];
            var battleGroup = new BattleGroupComponent();
            var statStorage = battleEntity.GetComponent(RoundUsersStatisticsStorageComponent.Id) as RoundUsersStatisticsStorageComponent;
            var battleComponent = battleEntity.GetComponent<BattleComponent>(BattleComponent.Id);
            lock(battleComponent.locker)
            {
                if (battleComponent.MaxPlayers > teamComp.Players)
                {
                    battleEntity.GetComponent<BattlePlayersComponent>(BattlePlayersComponent.Id).players.TryAdd(userEntity, teamComp);
                    RoundUserStatisticsComponent userStat = null;
                    if (!statStorage.roundUserStatisticsComponents.TryGetValue(userEntity, out userStat))
                    {
                        userStat = new RoundUserStatisticsComponent((battleEntity.GetComponent(BattlePlayersComponent.Id) as BattlePlayersComponent).players.Values.Where(x => x == teamComp).ToList().Count - 1, 0, 0, 0, 0, (userEntity.GetComponent(UsernameComponent.Id) as UsernameComponent).Username, (userEntity.GetComponent(UserRankComponent.Id) as UserRankComponent).Rank).SetGlobalComponentGroup().AddComponentGroup(battleGroup) as RoundUserStatisticsComponent;
                        statStorage.roundUserStatisticsComponents.TryAdd(userEntity, userStat);
                    }
                    
                    userEntity.AddComponents(new List<ECSComponent>()
                    {
                        new BattleOwnerComponent(battleEntity).SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                        GenerateBattleUserEquipment(userEntity, battleEntity).SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                        new TankCooldownStorageComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                        teamComp,
                       // new UserReadyToBattleComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                        new WorldPositionComponent(new Types.Battle.AtomicType.WorldPoint()).SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                        userStat.SetGlobalComponentGroup(),
                        new TankInBattleComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                        new TankMovableComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                        new TankMovementComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                        //new TankSpawnStateComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup)
                    });
                    new EquipmentTemplate().SetupEntity(userEntity);
                    foreach (var additionComponent in teamComp.ComponentsForPlayerAppend)
                    {
                        var addComp = additionComponent.Clone() as ECSComponent;
                        addComp.instanceId = Guid.NewGuid().GuidToLong();
                        userEntity.AddComponent(addComp.SetGlobalComponentGroup().AddComponentGroup(battleGroup));

                    }
                (userEntity.GetComponent(CharacteristicTransformersComponent.Id) as CharacteristicTransformersComponent).disabledTransformers = new System.Collections.Concurrent.ConcurrentDictionary<long, bool>(teamComp.DisabledCharacteristicTransformers.Select(x => KeyValuePair.Create(x.GetId(), true)).ToList());
                    (userEntity.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent).disabledEffects = new System.Collections.Concurrent.ConcurrentDictionary<long, bool>(teamComp.DisabledEffects.Select(x => KeyValuePair.Create(x.GetId(), true)).ToList());
                    (userEntity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent).disabledResistance = new System.Collections.Concurrent.ConcurrentDictionary<long, bool>(teamComp.DisabledResistance.Select(x => KeyValuePair.Create(x.GetId(), true)).ToList());
                    if(!battleEntity.HasComponent(DMComponent.Id))
                        userEntity.dataAccessPolicies.Add(teamComp.teamGDAP.Clone() as GroupDataAccessPolicy);
                    userEntity.dataAccessPolicies.Add((battleEntity.GetComponent(BattleGDAPComponent.Id) as BattleGDAPComponent).battleGDAP.Clone() as GroupDataAccessPolicy);
                    userEntity.dataAccessPolicies.Add(new BattleMemberGDAP());
                    (battleEntity.GetComponent(RoundUsersStatisticsStorageComponent.Id) as RoundUsersStatisticsStorageComponent).roundUserStatisticsComponents.TryAdd(userEntity, userEntity.GetComponent(RoundUserStatisticsComponent.Id) as RoundUserStatisticsComponent);
                    teamComp.Players++;
                }
            }
            //battleComponent.locker.ExitWriteLock();
            return null;
        }

        public static UserBattleGarageDBComponent GenerateBattleUserEquipment(ECSEntity userEntity, ECSEntity battleEntity, UpdateSupplyCountEvent updateSupplyCountEvent = null)
        {
            var userGarage = userEntity.GetComponent<UserGarageDBComponent>();
            var battleSettings = battleEntity.GetComponent<BattleComponent>();
            UserBattleGarageDBComponent battleGarage = null;
            bool presented = false;
            if (userEntity.HasComponent(UserBattleGarageDBComponent.Id))
            {
                battleGarage = userEntity.GetComponent<UserBattleGarageDBComponent>();
                presented = true;
            }
            else
                battleGarage = new UserBattleGarageDBComponent();

            battleGarage.battleEquipment = new Types.GarageData
            {
                Turrets = new List<Types.Turret>(userGarage.selectedEquipment.Turrets),
                Hulls = new List<Types.Hull>(userGarage.selectedEquipment.Hulls),
                Colormaps = new List<Types.Colormap>(userGarage.selectedEquipment.Colormaps),
                Modules = battleSettings.enableModules ? new List<Types.Module>(userGarage.selectedEquipment.Modules) : new List<Types.Module>(),
                Supplies = battleSettings.enablePlayerSupplies ? new List<Types.Supply>(userGarage.garage.Supplies) : new List<Types.Supply>()
            };
            if(updateSupplyCountEvent != null)
            {
                if (battleSettings.enablePlayerSupplies)
                {
                    updateSupplyCountEvent.SyncSupply = battleGarage.battleEquipment.Supplies;
                }
                else
                {
                    updateSupplyCountEvent.InBattleSupply = battleGarage.battleEquipment.Supplies;
                }
            }
            if (presented && updateSupplyCountEvent == null)
                battleGarage.MarkAsChanged();
            return battleGarage;
        }

        public ECSEntity ExitFromBatle(ECSEntity userEntity, ECSEntity battleEntity, long teamId, LeaveFromBattleEvent leaveFromBattleEvent)
        {
            lock (userEntity.contextSwitchLocker)
            {
                var statStorage = battleEntity.GetComponent(RoundUsersStatisticsStorageComponent.Id) as RoundUsersStatisticsStorageComponent;
                var creatureStorage = battleEntity.GetComponent<BattleCreatureStorageComponent>();
                creatureStorage.RemoveComponentsByType(new List<long> { MineCreatureComponent.Id }, new List<ECSEntity> { userEntity });
                creatureStorage.MarkAsChanged();
                var userStat = userEntity.GetComponent(RoundUserStatisticsComponent.Id) as RoundUserStatisticsComponent;
                var battlePlayers = (battleEntity.GetComponent(BattlePlayersComponent.Id) as BattlePlayersComponent);
                if (!battlePlayers.players.TryRemove(userEntity, out _))
                    Logger.LogError("error remove player from battle");
                userEntity.GetComponent<CharacteristicTransformersComponent>().characteristicTransformers.Values.ForEach((x) => {
                    if (!x.stableTransformer)
                        userEntity.TryRemoveComponent((x as ECSComponent).GetId());
                });
                foreach (var battlePlayer in battlePlayers.players)
                {
                    battlePlayer.Key.GetComponent<UserSocketComponent>().Socket.emit(leaveFromBattleEvent.PackToNetworkPacket());
                }

                if (!statStorage.roundUserStatisticsComponents.TryGetValue(userEntity, out _))
                {
                    statStorage.roundUserStatisticsComponents.TryAdd(userEntity, userStat);
                }
                var battlePlayerGDAPId = (battleEntity.GetComponent(BattleGDAPComponent.Id) as BattleGDAPComponent).battleGDAP.instanceId;
                
                var teamGDAPId = (userEntity.GetComponent(TeamComponent.Id) as TeamComponent).teamGDAP.instanceId;
                userEntity.GetComponent<TeamComponent>().Players--;
                userEntity.TryRemoveComponent(InBattleChangeWeaponTimeoutComponent.Id);
                List<GroupDataAccessPolicy> toDeleteList = new List<GroupDataAccessPolicy>();
                userEntity.dataAccessPolicies.ForEach((playergdap)=>
                {
                    if(playergdap.instanceId == battlePlayerGDAPId || playergdap.instanceId == teamGDAPId || playergdap.GetId() == BattleMemberGDAP.Id)
                    {
                        toDeleteList.Add(playergdap);
                    }
                });
                toDeleteList.ForEach((deletedGDAP) => userEntity.dataAccessPolicies.Remove(deletedGDAP));
                userEntity.RemoveComponentsWithGroup(new BattleGroupComponent());
                BattleInfoUpdaterSystem.BattleSimpleInfoUpdater(battleEntity);
            }
            EntitySerialization.SerializeEntity(userEntity, true);
            var entitySerialized = EntitySerialization.BuildSerializedEntityWithGDAP(userEntity, userEntity, true);

            if (entitySerialized.Item1 == "" && entitySerialized.Item2.Count == 0)
                return userEntity;

            var userSocket = userEntity.GetComponent(UserSocketComponent.Id) as UserSocketComponent;
            UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
            {
                EntityIdRecipient = userEntity.instanceId,
                Entities = new List<string>()
                {
                    entitySerialized.Item1
                }
            };

            Func<Task> asyncUpd = async () =>
            {
                await Task.Run(() => {
                    if (entitySerialized.Item1 != "")
                        userSocket.Socket.emit(updateEntitiesEvent.PackToNetworkPacket());
                    foreach (var rawEvent in entitySerialized.Item2)
                    {
                        userSocket.Socket.emit(rawEvent);
                    }
                });
            };
            asyncUpd();
            return userEntity;
        }

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }
    }
}
