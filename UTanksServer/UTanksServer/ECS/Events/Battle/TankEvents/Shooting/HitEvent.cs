using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle;

namespace UTanksServer.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(226492668540878530)]
    public class HitEvent : ECSEvent
    {
        static public new long Id { get; set; }
        // long is entity id
        public Dictionary<long, Vector3S> hitList;
        public Dictionary<long, float> hitDistanceList;
        public Dictionary<long, float> hitLocalDistanceList;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
