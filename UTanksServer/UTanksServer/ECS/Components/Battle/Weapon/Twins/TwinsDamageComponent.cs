using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
	[TypeUid(220429478090486370)]
	public class TwinsDamageComponent : WeaponComponent
	{
		static public new long Id { get; set; }
		static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float minDamageProperty { get; set; }
        public float maxDamageProperty { get; set; }
        public float reloadTimeProperty { get; set; }
        public float bulletSpeedProperty { get; set; }
        public float bulletRadiusProperty { get; set; }
        public float maxDamageDistanceProperty { get; set; }
        public float minDamageDistanceProperty { get; set; }
        public float minDamagePercentProperty { get; set; }
        public float impactProperty { get; set; }
        public float kickbackProperty { get; set; }

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            var twinsComponent = tankComponent as TwinsComponent;
            this.minDamageProperty = twinsComponent.minDamageProperty;
            this.maxDamageProperty = twinsComponent.maxDamageProperty;
            this.reloadTimeProperty = twinsComponent.reloadTimeProperty;
            this.bulletSpeedProperty = twinsComponent.bulletSpeedProperty;
            this.bulletRadiusProperty = twinsComponent.bulletRadiusProperty;
            this.maxDamageDistanceProperty = twinsComponent.maxDamageDistanceProperty;
            this.minDamageDistanceProperty = twinsComponent.minDamageDistanceProperty;
            this.minDamagePercentProperty = twinsComponent.minDamagePercentProperty;
            this.impactProperty = twinsComponent.impactProperty;
            this.kickbackProperty = twinsComponent.kickbackProperty;
            this.turretSpeedRotationProperty = twinsComponent.turretSpeedRotationProperty;
            this.ComponentGrade = twinsComponent.ComponentGrade;
            return this;
        }
    }
}
