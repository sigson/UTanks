using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(208198038328279870)]
    public class IsisDamageComponent : WeaponComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public float damagePerSecondProperty { get; set; }
		public float healingProperty { get; set; }
		public float energyChargeSpeedProperty { get; set; }
		public float energyRechargeSpeedProperty { get; set; }
		public float selfHealingProperty { get; set; }
		public float increaseFriendTemperatureProperty { get; set; }
		public float decreaseFriendTemperatureProperty { get; set; }
		public float maxShotDistanceProperty { get; set; }

		public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
			var isisComponent = tankComponent as IsisComponent;
			this.damagePerSecondProperty = isisComponent.damagePerSecondProperty;
			this.healingProperty = isisComponent.healingProperty;
			this.energyChargeSpeedProperty = isisComponent.energyChargeSpeedProperty;
			this.energyRechargeSpeedProperty = isisComponent.energyRechargeSpeedProperty;
			this.selfHealingProperty = isisComponent.selfHealingProperty;
			this.increaseFriendTemperatureProperty = isisComponent.increaseFriendTemperatureProperty;
			this.decreaseFriendTemperatureProperty = isisComponent.decreaseFriendTemperatureProperty;
			this.maxShotDistanceProperty = isisComponent.maxShotDistanceProperty;
			this.ComponentGrade = isisComponent.ComponentGrade;
			this.turretSpeedRotationProperty = isisComponent.turretSpeedRotationProperty;
			return this;
		}
	}
}
