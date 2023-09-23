using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.ECSEvents
{
    [TypeUidAttribute(192870015372175650)]
    public class CreatureUpdateEvent : ECSEvent //obsolete
    {
        static public new long Id { get; set; } = 192870015372175650;
        public long OwnerId;
        public Dictionary<long, List<ICreatureComponent>> UpdatedCreatures = new Dictionary<long, List<ICreatureComponent>>();
        public override void Execute()
        {
            ECSEntity dbOwner = null;
            if(ManagerScope.entityManager.EntityStorage.TryGetValue(OwnerId, out dbOwner))
            {
                if (dbOwner.HasComponent(BattleCreatureStorageComponent.Id))
                {
                    var storage = dbOwner.GetComponent<BattleCreatureStorageComponent>();
                    foreach (var updCreaturesEntity in UpdatedCreatures)
                    {
                        foreach (var updCreatures in UpdatedCreatures)
                        {

                        }
                    }
                }
            }
        }
    }
}
