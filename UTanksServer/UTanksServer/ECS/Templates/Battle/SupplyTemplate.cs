using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle.TankEvents;
using UTanksServer.Services;

namespace UTanksServer.ECS.Templates.Battle
{
    [TypeUid(174649274178405900)]
    public class SupplyTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 174649274178405900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public static void UseSupply(ECSEntity playerEntity, SupplyUsedEvent supplyUsedEvent, params string[] ConfigSupply)
        {
            var battleComponent = playerEntity.GetComponent<BattleOwnerComponent>().Battle.GetComponent<BattleComponent>();
            foreach (var supplyConf in ConfigSupply)
            {
                var supplyConfig = ConstantService.GetByConfigPath(supplyConf);
                List<ECSEntity> listTargets = new List<ECSEntity>();
                bool playerAdded = false;
                int targetCounter = 0;
                
                if (supplyConfig.Deserialized["playerSeparation"]["enabled"].ToString() == "true" && battleComponent.enablePlayerSupplySeparation)
                {
                    foreach (var supplyTarget in supplyUsedEvent.targetEntities)
                    {
                        ECSEntity targetEntity = null;
                        if(targetCounter + 1 == GlobalGameDataConfig.SelectableMap.selectableMaps.MaxPlayerSupplySeparation && !playerAdded)
                        {
                            playerAdded = true;
                            listTargets.Add(playerEntity);
                            break;
                        }
                        if (targetCounter == GlobalGameDataConfig.SelectableMap.selectableMaps.MaxPlayerSupplySeparation && playerAdded)
                            break;
                        if (supplyTarget != playerEntity.instanceId && ManagerScope.entityManager.EntityStorage.TryGetValue(supplyTarget, out targetEntity))
                        {
                            listTargets.Add(targetEntity);
                        }
                        else
                        {
                            playerAdded = true;
                            listTargets.Add(playerEntity);
                        }
                    }
                }
                else
                {
                    listTargets.Add(playerEntity);
                }
                
                if(battleComponent.enableSupplyCooldown)
                {
                    var supCoolDown = double.Parse(supplyConfig.Deserialized["usingSecCooldown"]["cooldown"].ToString(), CultureInfo.InvariantCulture);
                    if(supCoolDown > 0)
                        playerEntity.GetComponent<TankCooldownStorageComponent>().CooldownStorage[supplyConf] = DateTime.Now.AddSeconds(supCoolDown).Ticks;
                }

                for (int i = 0; i < supplyConfig.Deserialized["modificators"].Count(); i++)
                {
                    var modificatorPath = supplyConfig.Deserialized["modificators"][i]["modificator"].ToString();
                    listTargets.ForEach(x => ModificatorTemplate.CreateModificator(x, 1f / listTargets.Count, modificatorPath));
                }
                for (int i = 0; i < supplyConfig.Deserialized["creatures"].Count(); i++)
                {
                    var creaturePath = supplyConfig.Deserialized["creatures"][i]["creature"].ToString();
                    listTargets.ForEach(x => CreatureTemplate.CreateBattleCreature(x, 1f / listTargets.Count, supplyUsedEvent, creaturePath));
                    
                }
            }
            playerEntity.GetComponent<TankCooldownStorageComponent>().MarkAsChanged();
        }

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }
    }
}
