using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(4207390770640273134L)]
    public class VulcanWeaponComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public VulcanWeaponComponent() { }
        public VulcanWeaponComponent(float speedUpTime, float slowDownTime, float temperatureIncreasePerSec, float temperatureLimit, float temperatureHittingTime, float weaponTurnDecelerationCoeff, float targetHeatingMult)
        {
            SpeedUpTime = speedUpTime;
            SlowDownTime = slowDownTime;
            TemperatureIncreasePerSec = temperatureIncreasePerSec;
            TemperatureLimit = temperatureLimit;
            TemperatureHittingTime = temperatureHittingTime;
            WeaponTurnDecelerationCoeff = weaponTurnDecelerationCoeff;
            TargetHeatingMult = targetHeatingMult;
        }

        public float SpeedUpTime { get; set; }
        public float SlowDownTime { get; set; }
        public float TemperatureIncreasePerSec { get; set; }
        public float TemperatureLimit { get; set; }
        public float TemperatureHittingTime { get; set; }
        public float WeaponTurnDecelerationCoeff { get; set; }
        public float TargetHeatingMult { get; set; }
    }
}