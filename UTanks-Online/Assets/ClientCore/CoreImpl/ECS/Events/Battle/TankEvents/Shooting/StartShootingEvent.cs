using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(226275548248396060)]
    public class StartShootingEvent : ECSEvent
    {
        static public new long Id { get; set; }

        //public Dictionary<long, Vector>
        public override void Execute()
        {
            //
        }
    }
}
