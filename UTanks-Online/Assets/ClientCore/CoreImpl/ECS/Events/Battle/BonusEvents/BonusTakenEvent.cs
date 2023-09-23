using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.BonusEvents
{
    [TypeUid(197690785600097820)]
    public class BonusTakenEvent : ECSEvent // deprecated
    {
        static public new long Id { get; set; }
        public long DropId;
        public override void Execute()
        {
            //
        }
    }
}
