using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle
{
    [TypeUid(216814436710227360)]
    public class BattleEndEvent : ECSEvent
    {
        public long BattleEntity;
        public long TeamWinnerInstanceId;
        public Dictionary<long, int> RewardList;
        static public new long Id { get; set; }
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
