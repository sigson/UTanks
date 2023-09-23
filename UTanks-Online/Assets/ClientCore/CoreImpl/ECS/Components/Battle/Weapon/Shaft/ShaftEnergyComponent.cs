using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(1826384779893027508L)]
    public class ShaftEnergyComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ShaftEnergyComponent() { }
        public ShaftEnergyComponent(float unloadEnergyPerQuickShot, float possibleUnloadEnergyPerAimingShot,
            float unloadAimingEnergyPerSec, float reloadEnergyPerSec)
        {
            UnloadEnergyPerQuickShot = unloadEnergyPerQuickShot;
            PossibleUnloadEnergyPerAimingShot = possibleUnloadEnergyPerAimingShot;
            UnloadAimingEnergyPerSec = unloadAimingEnergyPerSec;
            ReloadEnergyPerSec = reloadEnergyPerSec;
        }

        public float UnloadEnergyPerQuickShot { get; set; }
        public float PossibleUnloadEnergyPerAimingShot { get; set; }
        public float UnloadAimingEnergyPerSec { get; set; }
        public float ReloadEnergyPerSec { get; set; }
    }
}
