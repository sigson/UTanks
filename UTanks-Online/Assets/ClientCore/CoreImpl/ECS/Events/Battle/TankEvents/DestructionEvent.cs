using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.TankEvents
{
    [TypeUid(167219879497699940)]
    public class DestructionEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public override void Execute()
        {
            //
        }
    }
}
