using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(-7212768015824297898L)]
	public class ShaftAimingSpeedComponent : ECSComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ShaftAimingSpeedComponent() { }
        public ShaftAimingSpeedComponent(float horizontalAcceleration, float maxHorizontalSpeed, float maxVerticalSpeed, float verticalAcceleration)
        {
            HorizontalAcceleration = horizontalAcceleration;
            MaxHorizontalSpeed = maxHorizontalSpeed;
            MaxVerticalSpeed = maxVerticalSpeed;
            VerticalAcceleration = verticalAcceleration;
        }

        public float HorizontalAcceleration { get; set; }

		public float MaxHorizontalSpeed { get; set; }

		public float MaxVerticalSpeed { get; set; }

		public float VerticalAcceleration { get; set; }
	}
}