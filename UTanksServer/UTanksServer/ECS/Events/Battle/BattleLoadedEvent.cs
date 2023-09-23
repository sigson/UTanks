using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle
{
    [TypeUid(219645388543217600)]
    public class BattleLoadedEvent : ECSEvent
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
