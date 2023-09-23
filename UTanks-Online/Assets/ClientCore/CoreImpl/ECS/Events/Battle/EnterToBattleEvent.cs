using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle
{
    [TypeUid(212689816075253220)]
    public class EnterToBattleEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long BattleId;
        public long TeamInstanceId;
        public override void Execute()
        {
            //
        }
    }
}
