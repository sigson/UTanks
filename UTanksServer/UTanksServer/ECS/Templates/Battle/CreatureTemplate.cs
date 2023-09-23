using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Bonus;
using UTanksServer.ECS.Components.Battle.CharacteristicTransformers;
using UTanksServer.ECS.Components.Battle.Creature;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle.TankEvents;
using UTanksServer.Services;
using static UTanksServer.ECS.ECSCore.ComponentsDBComponent;

namespace UTanksServer.ECS.Templates.Battle
{
    [TypeUid(203621007665540200)]
    public class CreatureTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 203621007665540200;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public static void CreateBattleCreature(ECSEntity playerEntity, float creatureCoef, SupplyUsedEvent supplyUsedEvent, params string[] ConfigCreature)
        {
            var configCreatures = new List<ConfigObj>();
            ConfigCreature.ForEach(x => configCreatures.Add(ConstantService.GetByConfigPath(x)));
            CreateBattleCreatureImpl(playerEntity, creatureCoef, supplyUsedEvent, configCreatures.ToArray());
        }

        public static void CreateBattleCreature(ECSEntity playerEntity, float creatureCoef, SupplyUsedEvent supplyUsedEvent, params ConfigObj[] ConfigCreature)
        {
            CreateBattleCreatureImpl(playerEntity, creatureCoef, supplyUsedEvent, ConfigCreature);
        }

        private static void CreateBattleCreatureImpl(ECSEntity playerEntity, float creatureCoef, SupplyUsedEvent supplyUsedEvent, ConfigObj[] ConfigCreature)
        {
            var battleOwner = playerEntity.GetComponent<BattleOwnerComponent>();
            var creatureDb = battleOwner.Battle.GetComponent<BattleCreatureStorageComponent>();
            Dictionary<long, (ECSComponent, ComponentState)> playerCreatures = null;
            if(!creatureDb.DB.TryGetValue(playerEntity.instanceId, out playerCreatures))
            {
                playerCreatures = new Dictionary<long, (ECSComponent, ComponentState)>();
                creatureDb.DB[playerEntity.instanceId] = playerCreatures;
            }
            List<ECSComponent> creaturesObjects = new List<ECSComponent>();
            foreach (var config in ConfigCreature)
            {
                switch(config.Path)
                {
                    case "battle\\creature\\mine":
                        if(true)
                        {
                            var creature = new MineCreatureComponent() {
                                minePoint = supplyUsedEvent.usingPoint == null ? new Types.Battle.AtomicType.WorldPoint() : supplyUsedEvent.usingPoint,
                                WorldPositon = supplyUsedEvent.usingPoint == null ? new Types.Battle.AtomicType.WorldPoint() : supplyUsedEvent.usingPoint,
                                mineState = MineState.Installed
                            };
                            var creatureStorage = battleOwner.Battle.GetComponent<BattleCreatureStorageComponent>();
                            creatureStorage.AddComponent(playerEntity, creature);
                            creatureStorage.MarkAsChanged();
                        }
                        break;
                    case "battle\\bonus\\gold\\crystal":
                        if (true)
                        {
                            var goldDropCount = battleOwner.Battle.GetComponent<BattleDropRegionsComponent>().dropRegions[Types.Battle.Bonus.BonusType.Gold].Count;
                            var dropStorage = ManagerScope.entityManager.EntityStorage[battleOwner.Battle.GetComponent<BattleDropRegionsComponent>().dropRegions[Types.Battle.Bonus.BonusType.Gold][new Random().Next(0, goldDropCount - 1)]].GetComponent<BonusRegionDropStorageComponent>();
                            dropStorage.DropStorage.Add(new BonusDropRecord(0, Types.Battle.Bonus.BonusType.Gold));
                        }
                        break;
                }
            }
            creaturesObjects.ForEach(x =>
            {
                if(x is ICreatureComponent)
                {

                }
            });
        }

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }
    }
}
