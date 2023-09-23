using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle;

namespace UTanksClient.ECS.Events.Battle.TankEvents
{
    [TypeUid(228997782483129570)]
    public class MoveCommandEvent : ECSEvent
    {
        static public new long Id { get; set; }

        public Vector3S position;
        public Vector3S velocity;
        public Vector3S angularVelocity;
        public QuaternionS rotation;
        public QuaternionS turretRotation;
        public float WeaponRotation { get; set; } //angle
        public float TankMoveControl { get; set; }
        public float TankTurnControl { get; set; }
        public float WeaponRotationControl { get; set; }

        public int ClientTime { get; set; }

        public override void Execute()
        {
            //
        }
    }
}
