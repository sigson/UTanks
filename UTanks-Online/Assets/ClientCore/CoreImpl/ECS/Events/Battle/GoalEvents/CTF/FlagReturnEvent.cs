using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.GoalEvents
{
    [TypeUid(171711023222354080)]
    public class FlagReturnEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long FlagEntityId;
        public override void Execute()
        {
            //
        }
    }
}
