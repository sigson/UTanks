using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle.GoalEvents
{
    [TypeUid(203914513634776220)]
    public class FlagTakenEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long FlagEntityId;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
