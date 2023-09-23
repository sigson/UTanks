using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle;

namespace UTanksClient.ECS.Events.Battle.BonusEvents
{
    [TypeUid(231854580708361900)]
    public class BonusTakingEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long DropId;
        public Vector3S contactPosition;
        public override void Execute()
        {
            //
        }
    }
}
