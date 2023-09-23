using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace Assets.ClientCore.CoreImpl.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUidAttribute(398421781583721756)]
    public class CreatureActuationEvent : ECSEvent
    {
        static public new long Id { get; set; } = 398421781583721756;
        public long BattleDBOwnerId;//battle
        public long CreatureInstanceId;
        public List<long> TargetsId;
        public override void Execute()
        {

        }
    }
}
