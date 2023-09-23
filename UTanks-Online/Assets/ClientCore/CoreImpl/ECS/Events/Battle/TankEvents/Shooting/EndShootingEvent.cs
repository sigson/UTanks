using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(208404642426789120)]
    public class EndShootingEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public override void Execute()
        {
            //
        }
    }
}
