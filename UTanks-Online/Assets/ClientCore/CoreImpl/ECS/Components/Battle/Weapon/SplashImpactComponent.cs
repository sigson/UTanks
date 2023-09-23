using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(1438773081827L)]
	public class SplashImpactComponent : ECSComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SplashImpactComponent() { }
        public SplashImpactComponent(float impactForce)
        {
            ImpactForce = impactForce;
        }

        public float ImpactForce { get; set; }
	}
}