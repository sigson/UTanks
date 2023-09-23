using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(227777235552527940)]
    public class StartChargingEvent : ECSEvent
    {
        static public new long Id { get; set; }

        public override void Execute()
        {
            //
        }
    }
}
