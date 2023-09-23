using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
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
            return null;
        }
    }
}
