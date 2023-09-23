//using UTanksClient.Core.Battles.BattleWeapons;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(1432792458422)]
    public class WeaponRotationComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WeaponRotationComponent() { }
        public WeaponRotationComponent(float simplifiedTurretRotation) =>
            Speed = Acceleration = BaseSpeed = simplifiedTurretRotation;

        //public void ChangeByTemperature(BattleWeapon battleWeapon, float temperatureMultiplier)
        //{
        //    float shaftAimingMultiplier = battleWeapon.GetType() == typeof(Shaft) && ((Shaft) battleWeapon).
        //        ShaftAimingBeginTime is not null ? Shaft.RotationAimingStateMultiplier : 1;
        //    Speed = battleWeapon.OriginalWeaponRotationComponent.Speed * shaftAimingMultiplier * temperatureMultiplier;
        //}

        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float BaseSpeed { get; set; }
    }
}
