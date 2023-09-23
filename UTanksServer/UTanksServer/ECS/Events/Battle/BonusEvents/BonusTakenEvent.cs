using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle.BonusEvents
{
    [TypeUid(197690785600097820)]
    public class BonusTakenEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long DropEntityId;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
