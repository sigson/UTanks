using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(1438152738643L)]
	public class WeaponBulletShotComponent : ECSComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WeaponBulletShotComponent() { }
        public WeaponBulletShotComponent(float bulletRadius, float bulletSpeed)
        {
            BulletRadius = bulletRadius;
            BulletSpeed = bulletSpeed;
        }

        public float BulletRadius { get; set; }

		public float BulletSpeed { get; set; }
	}
}