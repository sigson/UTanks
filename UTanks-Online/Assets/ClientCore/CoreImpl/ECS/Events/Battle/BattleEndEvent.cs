using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle
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
            //
        }
    }
}
