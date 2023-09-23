using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Network.NetworkEvents;

namespace UTanksServer.ECS.Components.Battle.Energy
{
    /// <summary>
    /// Energy of a weapon within 0..1 range.
    /// </summary>
    [TypeUid(8236491228938594733)]
    public class WeaponEnergyComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WeaponEnergyComponent() { 
            DirectiveUpdate = true;
        }
        public WeaponEnergyComponent(float maxEnergy)
        {
            Energy = maxEnergy;
            MaxEnergy = maxEnergy;
            DirectiveUpdate = true;
        }

        protected override void ImplDirectiveSerialization()
        {
            base.ImplDirectiveSerialization();
            DirectiveUpdateContainer = new RawEnergyComponentDirectUpdateEvent()
            {
                Energy = this.Energy,
                MaxEnergy = this.MaxEnergy,
                PlayerUpdateEntityId = this.ownerEntity.instanceId,
                ComponentInstanceId = this.instanceId
            };
        }

        public float Energy { get; set; } = 0;
        public float MaxEnergy { get; set; } = 0;
    }
}
