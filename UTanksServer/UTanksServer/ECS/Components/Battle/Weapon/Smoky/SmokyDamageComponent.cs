using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
	[TypeUid(212091282261699260)]
	public class SmokyDamageComponent : WeaponComponent
	{
		static public new long Id { get; set; }
		static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float minDamageProperty { get; set; }
        public float maxDamageProperty { get; set; }
        public float reloadTimeProperty { get; set; }
        public float criticalDamageProperty { get; set; }
        public float startCriticalProbabilityProperty { get; set; }
        public float afterCriticalHitProbabilityProperty { get; set; }
        public float maxCriticalProbabilityProperty { get; set; }
        public float criticalProbabilityDeltaProperty { get; set; }
        public float maxDamageDistanceProperty { get; set; }
        public float minDamageDistanceProperty { get; set; }
        public float minDamagePercentProperty { get; set; }
        public float impactProperty { get; set; }
        public float kickbackProperty { get; set; }

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            var smokyComponent = tankComponent as SmokyComponent;
            this.minDamageProperty = smokyComponent.minDamageProperty;
            this.maxDamageProperty = smokyComponent.maxDamageProperty;
            this.reloadTimeProperty = smokyComponent.reloadTimeProperty;
            this.criticalDamageProperty = smokyComponent.criticalDamageProperty;
            this.startCriticalProbabilityProperty = smokyComponent.startCriticalProbabilityProperty;
            this.afterCriticalHitProbabilityProperty = smokyComponent.afterCriticalHitProbabilityProperty;
            this.maxCriticalProbabilityProperty = smokyComponent.maxCriticalProbabilityProperty;
            this.criticalProbabilityDeltaProperty = smokyComponent.criticalProbabilityDeltaProperty;
            this.maxDamageDistanceProperty = smokyComponent.maxDamageDistanceProperty;
            this.minDamageDistanceProperty = smokyComponent.minDamageDistanceProperty;
            this.minDamagePercentProperty = smokyComponent.minDamagePercentProperty;
            this.impactProperty = smokyComponent.impactProperty;
            this.kickbackProperty = smokyComponent.kickbackProperty;
            this.ComponentGrade = smokyComponent.ComponentGrade;
            this.turretSpeedRotationProperty = smokyComponent.turretSpeedRotationProperty;
            return this;
        }
    }
}
