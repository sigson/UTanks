using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.ECSEvents
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
