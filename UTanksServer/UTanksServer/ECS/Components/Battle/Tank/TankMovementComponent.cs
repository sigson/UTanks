using System.Numerics;
using System.Runtime.InteropServices;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.ECS.Types.Battle.AtomicType;

namespace UTanksServer.ECS.Components.Battle.Tank
{
    [TypeUid(-615965945505672897)]
    public class TankMovementComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TankMovementComponent() { }
        public TankMovementComponent(Movement movement, MoveControl moveControl, float weaponRotation, float weaponControl)
        {
            Movement = movement;
            MoveControl = moveControl;
            WeaponRotation = weaponRotation;
            WeaponControl = weaponControl;
        }

        public Movement Movement { get; set; }

        public MoveControl MoveControl { get; set; }

        public float WeaponRotation { get; set; }

        public float WeaponControl { get; set; }
    }

    //[StructLayout(LayoutKind.Sequential, Size = 1)]
    public class Movement : CachingSerializable
    {
        public Movement(Vector3S position, Vector3S velocity, Vector3S angularVelocity, QuaternionS orientation)
        {
            Position = position;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
            Orientation = orientation;
        }

        public Movement()
        {

        }

        public Vector3S Position { get; set; }

        public Vector3S Velocity { get; set; }

        public Vector3S AngularVelocity { get; set; }

        public QuaternionS Orientation { get; set; }
    }

    //[StructLayout(LayoutKind.Sequential, Size = 1)]
    public class MoveControl : CachingSerializable
    {
        public MoveControl(float moveAxis, float turnAxis)
        {
            MoveAxis = moveAxis;
            TurnAxis = turnAxis;
        }

        public float MoveAxis { get; set; }

        public float TurnAxis { get; set; }
    }
}
