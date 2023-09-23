using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Round;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.DataAccessPolicy.Battles;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.ECS.Types.Battle.Team;
using UTanksServer.ECS.Types.Lobby;
using UTanksServer.Services;

namespace UTanksServer.ECS.Templates.Battle
{
    [TypeUid(216086468345602700)]
    public class BattleTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 207953786672822900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public List<ECSEntity> CreateBattleEntities(CreateBattleEvent createBattleEvent)
        {
            MapValue map;
            try
            {
                map = GlobalGameDataConfig.SelectableMap.selectableMaps.GameMaps.Where(map => map.Name == createBattleEvent.GameMapGroupName).ToList()[0].Maps.Where(mapgroup => mapgroup.Path == createBattleEvent.MapPath).ToList()[0];
            }
            catch
            {
                return new List<ECSEntity>();
                //ddos event
            }
            if((ManagerScope.entityManager.EntityStorage[createBattleEvent.EntityOwnerId].GetComponent(UserRankComponent.Id) as UserRankComponent).Rank < map.MinimalRankAccess)
            {
                return new List<ECSEntity>();
                //ddos event
            }
            int allRanks = ConstantService.GetByConfigPath("ranksconfig").Deserialized["ranksExperiencesConfig"]["ranksExperiences"].Count();
            if((createBattleEvent.BattleWinGoalValue < 1 && createBattleEvent.BattleTimeMinutes < 1) || createBattleEvent.MaxPlayers < 1 || createBattleEvent.MinimalPlayerRankValue < 0 || createBattleEvent.MinimalPlayerRankValue > allRanks || createBattleEvent.MaximalPlayerRankValue < 0 || createBattleEvent.MaximalPlayerRankValue > allRanks)
            {
                return new List<ECSEntity>();
                //ddos event
            }
            //write battle validator for check of restriction

            var roundGroup = new RoundGroupComponent();
            ECSComponent battleMode = (ECSComponent)Activator.CreateInstance(BattleTypes.Modes[createBattleEvent.BattleMode]);

            var battleGroup = new BattleGroupComponent();

            var BattleEntity = new ECSEntity(this, new ECSComponent[] {
                new BattleComponent()
                {
                    BattleMode = createBattleEvent.BattleMode,
                    BattleCustomName = createBattleEvent.BattleCustomName,
                    BattleRealName = createBattleEvent.BattleRealName,
                    MapPath = createBattleEvent.MapPath,
                    MapModel = createBattleEvent.MapModel,
                    MapConfigPath = createBattleEvent.MapConfigPath,
                    BattleTimeMinutes = createBattleEvent.BattleTimeMinutes,
                    BattleWinGoalValue = createBattleEvent.BattleWinGoalValue,
                    WeatherMode = createBattleEvent.WeatherMode,
                    TimeMode = createBattleEvent.TimeMode,
                    DamageScalingCoeficient = createBattleEvent.DamageScalingCoeficient,
                    HealthScalingCoeficient = createBattleEvent.HealthScalingCoeficient,
                    ListOfAcceptedConfigPathHull = createBattleEvent.ListOfAcceptedConfigPathHull,
                    ListOfAcceptedConfigPathWeapon = createBattleEvent.ListOfAcceptedConfigPathWeapon,
                    MaximalPlayerRankValue = createBattleEvent.MaximalPlayerRankValue,
                    MaxPlayers = createBattleEvent.MaxPlayers,
                    MinimalPlayerRankValue = createBattleEvent.MinimalPlayerRankValue,
                    LuminosityStrength = createBattleEvent.LuminosityStrength,
                    dressingUpTimeoutSeconds = createBattleEvent.dressingUpTimeoutSeconds,
                    enableAutoPeaceOnSuperDrop = createBattleEvent.enableAutoPeaceOnSuperDrop,
                    enableBattleAutoEnding = createBattleEvent.enableBattleAutoEnding,
                    enableCrystalDrop = createBattleEvent.enableCrystalDrop,
                    enableDressingUp = createBattleEvent.enableDressingUp,
                    enableMicroUpgrade = createBattleEvent.enableMicroUpgrade,
                    enablePlayerAutoBalance = createBattleEvent.enablePlayerAutoBalance,
                    enablePlayerSupplies = createBattleEvent.enablePlayerSupplies,
                    enableResists = createBattleEvent.enableResists,
                    enableSuperDrop = createBattleEvent.enableSuperDrop,
                    enableSupplyDrop = createBattleEvent.enableSupplyDrop,
                    enableTeamKilling = createBattleEvent.enableTeamKilling,
                    enableUnlimitedUserSupply = createBattleEvent.enableUnlimitedUserSupply,
                    enableModules = createBattleEvent.enableModules,
                    enablePlayerSupplySeparation = createBattleEvent.enablePlayerSupplySeparation,
                    enableSupplyCooldown = createBattleEvent.enableSupplyCooldown,
                    isCheatersBattle = createBattleEvent.isCheatersBattle,
                    isClosedBattle = createBattleEvent.isClosedBattle,
                    isParkourBattle = createBattleEvent.isParkourBattle,
                    isProBattle = createBattleEvent.isProBattle,
                    isTestBoxBattle = createBattleEvent.isTestBoxBattle,
                    isTournamentBattle = createBattleEvent.isTournamentBattle,
                    GravityScaling = createBattleEvent.GravityScaling,
                    MassScaling = createBattleEvent.MassScaling


                }.SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                new BattleSimpleInfoComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                new BattlePlayersComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                new RoundTimerComponent(createBattleEvent.BattleTimeMinutes).SetGlobalComponentGroup().AddComponentGroup(roundGroup),
                //new RoundTimerComponent(createBattleEvent.BattleTimeMinutes).SetGlobalComponentGroup().AddComponentGroup(roundGroup),
                new BattleScoreComponent().SetGlobalComponentGroup(),
                new RoundUsersStatisticsStorageComponent().SetGlobalComponentGroup().AddComponentGroup(roundGroup),
                battleMode.SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                new BattleGDAPComponent() {battleGDAP = new StandartBattlePlayerGDAP()}.SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                new BattleCreatureStorageComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                new BattleDropStorageComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup)
            });


            BattleTeamsComponent teams = null;
            List<ECSEntity> additionalEntities = new List<ECSEntity>();
            if (battleMode.GetId() == DMComponent.Id)
            {
                var teamComp = new TeamComponent() {
                    TeamColor = "dm",
                    teamGDAP = new TeamPlayerGDAP(),
                    FinalGoalValue = createBattleEvent.BattleWinGoalValue,
                    GoalScore = 0,
                    BattleEntityInstanceId = BattleEntity.instanceId
                }.SetGlobalComponentGroup().AddComponentGroup(battleGroup) as TeamComponent;
                teams = new BattleTeamsComponent() { 
                    teams = new ConcurrentDictionary<long, TeamComponent>(
                        new Dictionary<long, TeamComponent>() 
                        {
                            { teamComp.instanceId, teamComp }
                        }
                    )
                };
            }
            else if(battleMode.GetId() == CTFComponent.Id)
            {
                var redTeamComp = new TeamComponent()
                {
                    TeamColor = "red",
                    teamGDAP = new TeamPlayerGDAP(),
                    FinalGoalValue = createBattleEvent.BattleWinGoalValue,
                    GoalScore = 0,
                    BattleEntityInstanceId = BattleEntity.instanceId
                }.SetGlobalComponentGroup().AddComponentGroup(battleGroup) as TeamComponent;
                var blueTeamComp = new TeamComponent()
                {
                    TeamColor = "blue",
                    teamGDAP = new TeamPlayerGDAP(),
                    FinalGoalValue = createBattleEvent.BattleWinGoalValue,
                    GoalScore = 0,
                    BattleEntityInstanceId = BattleEntity.instanceId
                }.SetGlobalComponentGroup().AddComponentGroup(battleGroup) as TeamComponent;
                teams = new BattleTeamsComponent()
                {
                    teams = new ConcurrentDictionary<long, TeamComponent>(
                        new Dictionary<long, TeamComponent>()
                        {
                            { redTeamComp.instanceId, redTeamComp },
                            { blueTeamComp.instanceId, blueTeamComp }
                        }
                    )
                };
                var redFlagEntity = new ECSEntity(this, new ECSComponent[] {
                    new FlagComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                    new FlagPedestalComponent(map.GoalPositionPoints["ctfFlag"]["Flagred"][0].Position).SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                    new FlagPositionComponent(map.GoalPositionPoints["ctfFlag"]["Flagred"][0].Position).SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                    new FlagHomeStateComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                    redTeamComp,
                    new BattleOwnerComponent(BattleEntity).SetGlobalComponentGroup().AddComponentGroup(battleGroup)
                });
                var blueFlagEntity = new ECSEntity(this, new ECSComponent[] {
                    new FlagComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                    new FlagPedestalComponent(map.GoalPositionPoints["ctfFlag"]["Flagblue"][0].Position).SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                    new FlagPositionComponent(map.GoalPositionPoints["ctfFlag"]["Flagblue"][0].Position).SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                    new FlagHomeStateComponent().SetGlobalComponentGroup().AddComponentGroup(battleGroup),
                    blueTeamComp,
                    new BattleOwnerComponent(BattleEntity).SetGlobalComponentGroup().AddComponentGroup(battleGroup)
                });
                additionalEntities.Add(redFlagEntity);
                additionalEntities.Add(blueFlagEntity);
            }
            else if(battleMode.GetId() == TDMComponent.Id)
            {
                var redTeamComp = new TeamComponent()
                {
                    TeamColor = "red",
                    teamGDAP = new TeamPlayerGDAP(),
                    FinalGoalValue = createBattleEvent.BattleWinGoalValue,
                    GoalScore = 0,
                    BattleEntityInstanceId = BattleEntity.instanceId
                }.SetGlobalComponentGroup().AddComponentGroup(battleGroup) as TeamComponent;
                var blueTeamComp = new TeamComponent()
                {
                    TeamColor = "blue",
                    teamGDAP = new TeamPlayerGDAP(),
                    FinalGoalValue = createBattleEvent.BattleWinGoalValue,
                    GoalScore = 0,
                    BattleEntityInstanceId = BattleEntity.instanceId
                }.SetGlobalComponentGroup().AddComponentGroup(battleGroup) as TeamComponent;
                teams = new BattleTeamsComponent()
                {
                    teams = new ConcurrentDictionary<long, TeamComponent>(
                        new Dictionary<long, TeamComponent>()
                        {
                            { redTeamComp.instanceId, redTeamComp },
                            { blueTeamComp.instanceId, blueTeamComp }
                        }
                    )
                };
            }
            else if(battleMode.GetId() == DOMComponent.Id)
            {
                var redTeamComp = new TeamComponent()
                {
                    TeamColor = "red",
                    teamGDAP = new TeamPlayerGDAP(),
                    FinalGoalValue = createBattleEvent.BattleWinGoalValue,
                    GoalScore = 0,
                    BattleEntityInstanceId = BattleEntity.instanceId
                }.SetGlobalComponentGroup().AddComponentGroup(battleGroup) as TeamComponent;
                var blueTeamComp = new TeamComponent()
                {
                    TeamColor = "blue",
                    teamGDAP = new TeamPlayerGDAP(),
                    FinalGoalValue = createBattleEvent.BattleWinGoalValue,
                    GoalScore = 0,
                    BattleEntityInstanceId = BattleEntity.instanceId
                }.SetGlobalComponentGroup().AddComponentGroup(battleGroup) as TeamComponent;
                teams = new BattleTeamsComponent()
                {
                    teams = new ConcurrentDictionary<long, TeamComponent>(
                        new Dictionary<long, TeamComponent>()
                        {
                            { redTeamComp.instanceId, redTeamComp },
                            { blueTeamComp.instanceId, blueTeamComp }
                        }
                    )
                };
            }
            foreach (var team in teams.teams)
            {
                team.Value.ComponentsForPlayerAppend = new List<ECSComponent>();//effects, modificators
                team.Value.DisabledCharacteristicTransformers = new List<ECSComponent>();
                team.Value.DisabledEffects = new List<ECSComponent>();
                team.Value.DisabledResistance = new List<ECSComponent>();
            }
            
            BattleEntity.AddComponentSilent(teams.SetGlobalComponentGroup().AddComponentGroup(battleGroup));
            var mapComp = new MapComponent()
            {
                map = map
            }.SetGlobalComponentGroup().AddComponentGroup(battleGroup) as MapComponent;
            BattleEntity.dataAccessPolicies.Add(new BattleLobbyGDAP());
            BattleEntity.dataAccessPolicies.Add(new BattleMemberGDAP());
            BattleEntity.AddComponentSilent(mapComp);
            BattleEntity.AddComponentSilent(new BonusDropTemplate().CreateDropRegions(mapComp, BattleEntity));
            BattleEntity.AddComponentSilent(new RoundFundComponent(mapComp).SetGlobalComponentGroup().AddComponentGroup(roundGroup));
            BattleEntity.AddComponentSilent(new BattleGameplayEntitiesComponent() {GameplayEntities = additionalEntities.Select(x => x.instanceId).ToList()});
            additionalEntities.Add(BattleEntity);
            return additionalEntities;
        }

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }
    }
}
