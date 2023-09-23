using UTanksClient.Core.Protocol;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(1438077188268L)]
    public class DiscreteWeaponEnergyComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public DiscreteWeaponEnergyComponent() { }

        public bool ReloadNoWaitFullEnergyConsumption { get; set; }//true example - ricochet, shaft, false example - hammer

        public float ReloadEnergyPerSec { get; set; }

        public float UnloadEnergyPerShot { get; set; }

        public bool ReloadingAfterZeroEnergy;
    }
}