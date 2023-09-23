using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.ECSEvents
{
    [TypeUidAttribute(221198248719315900)]
    public class UpdateBattleCreaturesEvent : ECSEvent
    {
        static public new long Id { get; set; } = 221198248719315900;
        public long BattleId;
        public Dictionary<long, List<ICreatureComponent>> appendCreatures = new Dictionary<long, List<ICreatureComponent>>();
        public Dictionary<long, List<long>> removeCreatures = new Dictionary<long, List<long>>();
        public override void Execute()
        {
            //foreach (var entity in Entities)
            //{
            //    //entity.entityComponents.RestoreComponentsAfterSerialization(entity);
            //    EntitySerialization.UpdateDeserialize(entity);
            //}
        }
    }
}
