using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle.TankEvents
{
    [TypeUid(207476992139568830)]
    public class SelfDestructionRequestEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
