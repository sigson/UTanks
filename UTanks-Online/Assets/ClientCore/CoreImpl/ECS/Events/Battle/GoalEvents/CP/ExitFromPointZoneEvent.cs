using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.GoalEvents.CP
{
    [TypeUid(208347151889083840)]
    public class ExitFromPointZoneEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long PointEntityId;
        public override void Execute()
        {
            //
        }
    }
}
