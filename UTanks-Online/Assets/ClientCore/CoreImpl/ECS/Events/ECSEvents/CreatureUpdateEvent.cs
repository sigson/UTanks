using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;

namespace Assets.ClientCore.CoreImpl.ECS.Events.ECSEvents
{
    [TypeUidAttribute(192870015372175650)]
    public class CreatureUpdateEvent : ECSEvent //obsolete
    {
        static public new long Id { get; set; } = 192870015372175650;
        public long OwnerId;
        public Dictionary<long, List<ICreatureComponent>> UpdatedCreatures = new Dictionary<long, List<ICreatureComponent>>();
        public override void Execute()
        {

        }
    }
}
