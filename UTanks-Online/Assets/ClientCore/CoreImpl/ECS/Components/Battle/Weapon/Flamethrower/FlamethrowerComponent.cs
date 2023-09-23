using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(-748006331130499044L)]
	public class FlamethrowerComponent : WeaponComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

		public float damagePerSecondProperty { get; set; }
		public float heatDamageProperty { get; set; }
		public float energyChargeSpeedProperty { get; set; }
		public float energyRechargeSpeedProperty { get; set; }
		public float temperatureLimitProperty { get; set; }
		public float deltaTemperaturePerSecondProperty { get; set; }
		public float maxDamageDistanceProperty { get; set; }
		public float minDamageDistanceProperty { get; set; }
		public float minDamagePercentProperty { get; set; }

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            throw new System.NotImplementedException();
        }
    }
}