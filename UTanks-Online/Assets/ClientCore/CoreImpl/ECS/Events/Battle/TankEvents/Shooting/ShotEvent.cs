using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle;

namespace UTanksClient.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(237743993477676380)]
    public class ShotEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public Vector3S StartGlobalPosition;
        public QuaternionS StartGlobalRotation;
        public Vector3S MoveDirectionNormalized;
        public Dictionary<long, Vector3S> hitList;//for momental damage weapon
        public Dictionary<long, float> hitDistanceList;//for momental damage weapon
        public Dictionary<long, float> hitLocalDistanceList;//splash
        public override void Execute()
        {
            //
        }
    }
}
