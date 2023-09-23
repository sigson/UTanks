using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle
{
    [TypeUid(196306241721110620)]
    class BattleStartEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public override void Execute()
        {
            //
        }
    }
}
