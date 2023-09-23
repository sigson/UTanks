using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle.TankEvents
{
    [TypeUid(180207522780340300)]
    public class KillEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long BattleId;
        public long WhoKilledId;
        public long WhoDeadId;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
