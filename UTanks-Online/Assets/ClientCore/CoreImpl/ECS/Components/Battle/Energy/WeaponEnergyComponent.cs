using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Energy
{
    /// <summary>
    /// Energy of a weapon within 0..1 range.
    /// </summary>
    [TypeUid(8236491228938594733)]
    public class WeaponEnergyComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WeaponEnergyComponent() { }
        public WeaponEnergyComponent(float maxEnergy)
        {
            Energy = maxEnergy;
            MaxEnergy = maxEnergy;
        }

        public float Energy { get; set; } = 0;
        public float MaxEnergy { get; set; } = 0;
    }
}
