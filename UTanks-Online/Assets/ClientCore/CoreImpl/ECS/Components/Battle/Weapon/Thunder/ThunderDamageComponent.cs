using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
	[TypeUid(187351338234411140)]
	public class ThunderDamageComponent : WeaponComponent
	{
		static public new long Id { get; set; }
		static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float minDamageProperty { get; set; }
        public float maxDamageProperty { get; set; }
        public float reloadTimeProperty { get; set; }
        public float maxDamageDistanceProperty { get; set; }
        public float minDamageDistanceProperty { get; set; }
        public float minDamagePercentProperty { get; set; }
        public float impactProperty { get; set; }
        public float splashImpactProperty { get; set; }
        public float kickbackProperty { get; set; }
        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            var thunderComponent = tankComponent as ThunderComponent;
            this.minDamageProperty = thunderComponent.minDamageProperty;
            this.maxDamageProperty = thunderComponent.maxDamageProperty;
            this.reloadTimeProperty = thunderComponent.reloadTimeProperty;
            this.splashImpactProperty = thunderComponent.splashImpactProperty;
            this.maxDamageDistanceProperty = thunderComponent.maxDamageDistanceProperty;
            this.minDamageDistanceProperty = thunderComponent.minDamageDistanceProperty;
            this.minDamagePercentProperty = thunderComponent.minDamagePercentProperty;
            this.impactProperty = thunderComponent.impactProperty;
            this.kickbackProperty = thunderComponent.kickbackProperty;
            this.ComponentGrade = thunderComponent.ComponentGrade;
            this.turretSpeedRotationProperty = thunderComponent.turretSpeedRotationProperty;
            return this;
        }
    }
}
