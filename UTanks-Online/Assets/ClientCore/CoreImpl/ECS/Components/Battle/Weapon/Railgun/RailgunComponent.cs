using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
	[TypeUid(1105463674216064116L)]
	public class RailgunComponent : WeaponComponent
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
            throw new System.NotImplementedException();
        }
    }
}