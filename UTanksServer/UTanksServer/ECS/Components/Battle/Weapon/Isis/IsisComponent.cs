using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(-8080087598394650833L)]
	public class IsisComponent : WeaponComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

		public float damagePerSecondProperty { get; set; }
		public float damagePerHitProperty => damagePerSecondProperty / 4;
		public float healingProperty { get; set; }
		public float energyChargeSpeedProperty { get; set; }
		public float energyChargeSpeedPerTimeProperty => energyChargeSpeedProperty / 4;
		public float energyRechargeSpeedProperty { get; set; }
		public float energyRechargeSpeedPerTimeProperty => energyRechargeSpeedProperty / 4;
		public float selfHealingProperty { get; set; }
		public float increaseFriendTemperatureProperty { get; set; }
		public float decreaseFriendTemperatureProperty { get; set; }
		public float maxShotDistanceProperty { get; set; }

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            throw new System.NotImplementedException();
        }
    }
}