using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(3169143415222756957L)]
	public class SplashWeaponComponent : ECSComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SplashWeaponComponent() { }
        public SplashWeaponComponent(float minSplashDamagePercent, float radiusOfMaxSplashDamage, float radiusOfMinSplashDamage)
        {
            MinSplashDamagePercent = minSplashDamagePercent;
            RadiusOfMaxSplashDamage = radiusOfMaxSplashDamage;
            RadiusOfMinSplashDamage = radiusOfMinSplashDamage;
        }

        public float MinSplashDamagePercent { get; set; }

		public float RadiusOfMaxSplashDamage { get; set; }

		public float RadiusOfMinSplashDamage { get; set; }
	}
}