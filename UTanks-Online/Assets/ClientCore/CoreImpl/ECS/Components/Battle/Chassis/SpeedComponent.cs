using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Chassis
{
    [TypeUid(-1745565482362521070)]
    public class SpeedComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SpeedComponent() { }
        public SpeedComponent(float speed, float turnSpeed, float acceleration)
        {
            Speed = speed;
            TurnSpeed = turnSpeed;
            Acceleration = acceleration;
        }

        public float Speed { get; set; }

        public float TurnSpeed { get; set; }

        public float Acceleration { get; set; }
    }
}