using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(2869455602943064305L)]
	public class DamageWeakeningByDistanceComponent : ECSComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public DamageWeakeningByDistanceComponent() { }
        public DamageWeakeningByDistanceComponent(float minDamagePercent, float radiusOfMaxDamage, float radiusOfMinDamage)
        {
            MinDamagePercent = minDamagePercent;
            RadiusOfMaxDamage = radiusOfMaxDamage;
            RadiusOfMinDamage = radiusOfMinDamage;
        }

        public float MinDamagePercent { get; set; }

		public float RadiusOfMaxDamage { get; set; }

		public float RadiusOfMinDamage { get; set; }
	}
}