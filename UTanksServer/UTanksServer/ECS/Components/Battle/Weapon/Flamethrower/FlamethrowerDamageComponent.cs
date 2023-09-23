using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
	[TypeUid(189314454223633800)]
	public class FlamethrowerDamageComponent : WeaponComponent
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

		public float damagePerHitProperty => damagePerSecondProperty / 4;
		public float energyChargeSpeedPerTimeProperty => energyChargeSpeedProperty / 4;
		public float energyRechargeSpeedPerTimeProperty => energyRechargeSpeedProperty / 4;

		public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
			var flameComponent = tankComponent as FlamethrowerComponent;
			ComponentGrade = flameComponent.ComponentGrade;
			damagePerSecondProperty = flameComponent.damagePerSecondProperty;
			deltaTemperaturePerSecondProperty = flameComponent.deltaTemperaturePerSecondProperty;
			energyChargeSpeedProperty = flameComponent.energyChargeSpeedProperty;
			energyRechargeSpeedProperty = flameComponent.energyRechargeSpeedProperty;
			heatDamageProperty = flameComponent.heatDamageProperty;
			maxDamageDistanceProperty = flameComponent.maxDamageDistanceProperty;
			minDamageDistanceProperty = flameComponent.minDamageDistanceProperty;
			minDamagePercentProperty = flameComponent.minDamagePercentProperty;
			temperatureLimitProperty = flameComponent.temperatureLimitProperty;
			turretSpeedRotationProperty = flameComponent.turretSpeedRotationProperty;
			return this;
		}
    }
}
