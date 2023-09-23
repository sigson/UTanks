using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle
{
    [TypeUid(212288179196248830)]
    public class RemoveBattleEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long BattleId;
        public override void Execute()
        {
            //
        }
    }
}
