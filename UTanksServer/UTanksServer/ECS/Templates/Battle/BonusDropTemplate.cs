using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Bonus;
using UTanksServer.ECS.Components.Battle.CharacteristicTransformers;
using UTanksServer.ECS.Components.ECSComponents;
using UTanksServer.ECS.DataAccessPolicy.Battles;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle.Bonus;

namespace UTanksServer.ECS.Templates.Battle
{
    [TypeUid(209316286945445700)]
    public class BonusDropTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 207953786672822900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public BattleDropRegionsComponent CreateDropRegions(MapComponent mapComponent, ECSEntity battle)
        {
            BattleDropRegionsComponent bonusDropZones = new BattleDropRegionsComponent();
            var battleComponent = battle.GetComponent<BattleComponent>();
            var battleDropStorage = battle.GetComponent<BattleDropStorageComponent>();
            foreach (var bonusZoneGroup in mapComponent.map.DropSpawnPoints)
            {
                foreach (var bonusZone in bonusZoneGroup.Value) 
                {
                    var MatchedBonusType = BonusMatching.Bonuses[bonusZoneGroup.Key];
                    #region battlesettingscheck
                    if (MatchedBonusType == BonusType.Armor ||
                                    MatchedBonusType == BonusType.Damage ||
                                    MatchedBonusType == BonusType.Repair ||
                                    MatchedBonusType == BonusType.Speed)
                    {
                        if (!battleComponent.enableSupplyDrop)
                            continue;
                    }
                    //if (MatchedBonusType == BonusType.Container ||
                    //                MatchedBonusType == BonusType.Crystal)
                    //{
                    //    if (!battleComponent.enableCrystalDrop)
                    //        continue;
                    //}
                    //if (MatchedBonusType == BonusType.Gold ||
                    //                MatchedBonusType == BonusType.Ruby ||
                    //                MatchedBonusType == BonusType.SuperContainer)
                    //{
                    //    if (!battleComponent.enableSuperDrop)
                    //        continue;
                    //}
                    if (MatchedBonusType == BonusType.TestBox)
                    {
                        if (!battleComponent.isTestBoxBattle)
                            continue;
                    }
                    #endregion
                    var MatchedBonusConfigPath = BonusMatching.ConfigBonuses[MatchedBonusType];
                    ECSEntity entity = new ECSEntity(this, new ECSComponent[] {
                        new BonusRegionComponent(MatchedBonusType, MatchedBonusConfigPath){ position = bonusZone.worldPosition, MaxPoint = bonusZone.Max, MinPoint = bonusZone.Min},
                        new BattleOwnerComponent(battle)
                    });
                    #region varcheck
                    bool cycle = true;
                    Action<ECSEntity> ondrop;


                    BonusRegionDropStorageComponent storageComp = new BonusRegionDropStorageComponent();
                    ondrop = (entity) =>
                    {
                        var regionStorage = entity.GetComponent(BonusRegionDropStorageComponent.Id) as BonusRegionDropStorageComponent;
                        for (int i = 0; i < regionStorage.DropStorage.Count; i++)
                        {
                            battleDropStorage.AddComponent(battle, BonusDropTemplate.CreateDrop(entity));
                        }
                        battleDropStorage.MarkAsChanged();
                    };
                    if (MatchedBonusType == BonusType.Armor ||
                                    MatchedBonusType == BonusType.Damage ||
                                    MatchedBonusType == BonusType.Repair ||
                                    MatchedBonusType == BonusType.Speed)
                    {
                        storageComp.DropStorage.Add(new BonusDropRecord(0, MatchedBonusType));
                    }
                    else if (MatchedBonusType == BonusType.TestBox)
                    {
                        storageComp.DropStorage.Add(new BonusDropRecord(0, MatchedBonusType));
                    }
                    else if (MatchedBonusType == BonusType.Crystal)
                    {
                        ondrop = (entity) =>
                        {
                            var regionStorage = entity.GetComponent(BonusRegionDropStorageComponent.Id) as BonusRegionDropStorageComponent;
                            for (int i = 0; i < regionStorage.DropStorage.Count; i++)
                            {
                                battleDropStorage.AddComponent(battle, BonusDropTemplate.CreateDrop(entity));
                            }
                            battleDropStorage.MarkAsChanged();
                            regionStorage.DropStorage.Clear();
                        };
                    }
                    else if (MatchedBonusType == BonusType.Container)
                    {
                        ondrop = (entity) =>
                        {
                            var regionStorage = entity.GetComponent(BonusRegionDropStorageComponent.Id) as BonusRegionDropStorageComponent;
                            for (int i = 0; i < regionStorage.DropStorage.Count; i++)
                            {
                                battleDropStorage.AddComponent(battle, BonusDropTemplate.CreateDrop(entity));
                            }
                            battleDropStorage.MarkAsChanged();
                            regionStorage.DropStorage.Clear();
                        };
                    }
                    else if (MatchedBonusType == BonusType.Gold ||
                        MatchedBonusType == BonusType.Ruby ||
                        MatchedBonusType == BonusType.SuperContainer)
                    {
                        ondrop = (entity) =>
                        {
                            var regionStorage = entity.GetComponent(BonusRegionDropStorageComponent.Id) as BonusRegionDropStorageComponent;
                            for (int i = 0; i < regionStorage.DropStorage.Count; i++)
                            {
                                battleDropStorage.AddComponent(battle, BonusDropTemplate.CreateDrop(entity));
                            }
                            battleDropStorage.MarkAsChanged();
                            regionStorage.DropStorage.Clear();
                        };
                    }
                    #endregion
                    entity.AddComponentSilent(new BonusDropTimeComponent(bonusZone.DroppingPeriod, cycle, ondrop));
                    entity.AddComponentSilent(storageComp);
                    if(bonusDropZones.dropRegions.TryGetValue(MatchedBonusType, out _))
                    {
                        bonusDropZones.dropRegions[MatchedBonusType].Add(entity.instanceId);
                    }
                    else
                    {
                        bonusDropZones.dropRegions.TryAdd(MatchedBonusType, new List<long>() { entity.instanceId });
                    }
                    ManagerScope.entityManager.OnAddNewEntity(entity);
                }
            }
            return bonusDropZones;
        }

        public static BonusComponent CreateDrop(ECSEntity bonusRegion)
        {
            var bonusRegionComp = bonusRegion.GetComponent<BonusRegionComponent>(BonusRegionComponent.Id);
            BonusComponent bonusComponent = null;
            if (bonusRegionComp.Type == BonusType.Speed)
            {
                bonusComponent = new BonusComponent(GlobalGameDataConfig.SelectableMap.selectableMaps.SupplyDropBonusDespawnSecTime)
                {
                    bonusState = BonusState.Dropped,
                    BonusConfig = bonusRegionComp.BonusConfig,
                    bonusType = bonusRegionComp.Type,
                    position = bonusRegionComp.position.Position,
                    BonusEra = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[2],
                    BonusVersion = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[3],
                    DateTimeDropped = DateTime.Now.Ticks
                };
                bonusComponent.CharacteristicTransformerContainer.characteristicTransformerComponent.Add(new NitroTransformerComponent(true));
            }
            if (bonusRegionComp.Type == BonusType.Armor)
            {
                bonusComponent = new BonusComponent(GlobalGameDataConfig.SelectableMap.selectableMaps.SupplyDropBonusDespawnSecTime)
                {
                    bonusState = BonusState.Dropped,
                    BonusConfig = bonusRegionComp.BonusConfig,
                    bonusType = bonusRegionComp.Type,
                    position = bonusRegionComp.position.Position,
                    BonusEra = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[2],
                    BonusVersion = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[3],
                    DateTimeDropped = DateTime.Now.Ticks
                };
                bonusComponent.CharacteristicTransformerContainer.characteristicTransformerComponent.Add(new ArmorTransformerComponent(true));
            }
            if (bonusRegionComp.Type == BonusType.Repair)
            {
                bonusComponent = new BonusComponent(GlobalGameDataConfig.SelectableMap.selectableMaps.SupplyDropBonusDespawnSecTime)
                {
                    bonusState = BonusState.Dropped,
                    BonusConfig = bonusRegionComp.BonusConfig,
                    bonusType = bonusRegionComp.Type,
                    position = bonusRegionComp.position.Position,
                    BonusEra = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[2],
                    BonusVersion = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[3],
                    DateTimeDropped = DateTime.Now.Ticks
                };
                bonusComponent.CharacteristicTransformerContainer.characteristicTransformerComponent.Add(new RepairTransformerComponent(true));
            }
            if (bonusRegionComp.Type == BonusType.Damage)
            {
                bonusComponent = new BonusComponent(GlobalGameDataConfig.SelectableMap.selectableMaps.SupplyDropBonusDespawnSecTime)
                {
                    bonusState = BonusState.Dropped,
                    BonusConfig = bonusRegionComp.BonusConfig,
                    bonusType = bonusRegionComp.Type,
                    position = bonusRegionComp.position.Position,
                    BonusEra = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[2],
                    BonusVersion = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[3],
                    DateTimeDropped = DateTime.Now.Ticks
                };
                bonusComponent.CharacteristicTransformerContainer.characteristicTransformerComponent.Add(new DamageTransformerComponent(true));
            }
            if (bonusRegionComp.Type == BonusType.Crystal || bonusRegionComp.Type == BonusType.Container)
            {
                bonusComponent = new BonusComponent(GlobalGameDataConfig.SelectableMap.selectableMaps.CrystalDropBonusDespawnSecTime)
                {
                    bonusState = BonusState.Dropped,
                    BonusConfig = bonusRegionComp.BonusConfig,
                    bonusType = bonusRegionComp.Type,
                    position = bonusRegionComp.position.Position,
                    BonusEra = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[2],
                    BonusVersion = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[3],
                    DateTimeDropped = DateTime.Now.Ticks
                };
            }
            if (bonusRegionComp.Type == BonusType.Gold || bonusRegionComp.Type == BonusType.SuperContainer)
            {
                bonusComponent = new BonusComponent(GlobalGameDataConfig.SelectableMap.selectableMaps.GoldDropBonusDespawnSecTime)
                {
                    bonusState = BonusState.Dropped,
                    BonusConfig = bonusRegionComp.BonusConfig,
                    bonusType = bonusRegionComp.Type,
                    position = bonusRegionComp.position.Position,
                    BonusEra = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[2],
                    BonusVersion = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[3],
                    DateTimeDropped = DateTime.Now.Ticks
                };
            }
            if (bonusRegionComp.Type == BonusType.Ruby)
            {
                bonusComponent = new BonusComponent(GlobalGameDataConfig.SelectableMap.selectableMaps.RubyDropBonusDespawnSecTime)
                {
                    bonusState = BonusState.Dropped,
                    BonusConfig = bonusRegionComp.BonusConfig,
                    bonusType = bonusRegionComp.Type,
                    position = bonusRegionComp.position.Position,
                    BonusEra = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[2],
                    BonusVersion = GlobalGameDataConfig.SelectableMap.selectableMaps.DropBonusSkinPath.Split('\\')[3],
                    DateTimeDropped = DateTime.Now.Ticks
                };
            }
            //dropEntity.dataAccessPolicies.Add(new DropBonuseGDAP());
            return bonusComponent;
        }

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }
    }
}
