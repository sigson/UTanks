using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle
{
    [TypeUid(196306241721110620)]
    public class BattleStartEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long BattleId;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
