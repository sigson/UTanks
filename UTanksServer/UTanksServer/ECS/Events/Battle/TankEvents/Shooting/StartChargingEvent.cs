﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(227777235552527940)]
    public class StartChargingEvent : ECSEvent
    {
        static public new long Id { get; set; }

        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
