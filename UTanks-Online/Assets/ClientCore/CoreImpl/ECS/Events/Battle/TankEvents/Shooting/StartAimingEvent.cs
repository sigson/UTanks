using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(236081252122267300)]
    public class StartAimingEvent : ECSEvent
    {
        static public new long Id { get; set; }

        public override void Execute()
        {
            //
        }
    }
}
