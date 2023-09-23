using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(242632972981530940)]
    public class ShotHPResultEvent : ECSEvent
    {
        public float Damage = 0f;
        public float Heal = 0f;
        public float Critical = 0f;
        public long StruckEntityId = 0;
        static public new long Id { get; set; }
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
