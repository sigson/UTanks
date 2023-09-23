using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
	[TypeUid(214619732516052930)]
	public class RailgunDamageComponent : WeaponComponent
	{
		static public new long Id { get; set; }
		static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float minDamageProperty { get; set; }
        public float maxDamageProperty { get; set; }
        public float chargeTimeProperty { get; set; }
        public float reloadTimeProperty { get; set; }
        public float damageWeakeningByTargetProperty { get; set; }
        public float impactProperty { get; set; }
        public float kickbackProperty { get; set; }
        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            var thunderComponent = tankComponent as RailgunComponent;
            this.minDamageProperty = thunderComponent.minDamageProperty;
            this.maxDamageProperty = thunderComponent.maxDamageProperty;
            this.reloadTimeProperty = thunderComponent.reloadTimeProperty;
            this.damageWeakeningByTargetProperty = thunderComponent.damageWeakeningByTargetProperty;
            this.chargeTimeProperty = thunderComponent.chargeTimeProperty;
            this.impactProperty = thunderComponent.impactProperty;
            this.kickbackProperty = thunderComponent.kickbackProperty;
            this.ComponentGrade = thunderComponent.ComponentGrade;
            this.turretSpeedRotationProperty = thunderComponent.turretSpeedRotationProperty;
            return this;
        }
    }
}
