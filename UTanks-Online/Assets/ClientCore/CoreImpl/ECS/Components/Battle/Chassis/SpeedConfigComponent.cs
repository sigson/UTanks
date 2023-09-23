using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Chassis
{
    [TypeUid(-177474741853856725L)]
    public class SpeedConfigComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SpeedConfigComponent() { }
        public SpeedConfigComponent(float turnAcceleration, float sideAcceleration, float reverseAcceleration, float reverseTurnAcceleration)
        {
            ReverseAcceleration = reverseAcceleration;
            ReverseTurnAcceleration = reverseTurnAcceleration;
            SideAcceleration = sideAcceleration;
            TurnAcceleration = turnAcceleration;
        }

        public float ReverseAcceleration { get; set; }

        public float ReverseTurnAcceleration { get; set; }

        public float SideAcceleration { get; set; }

        public float TurnAcceleration { get; set; }
    }
}