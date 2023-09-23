using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle
{
    [TypeUid(218039566410825700)]
    public class LeaveFromBattleEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long BattleId;
        public long TeamInstanceId;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
