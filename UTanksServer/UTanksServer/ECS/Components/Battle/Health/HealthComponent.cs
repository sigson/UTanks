using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Network.NetworkEvents;

namespace UTanksServer.ECS.Components.Battle.Health
{
    [TypeUid(1949198098578360952)]
    public class HealthComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public HealthComponent() { DirectiveUpdate = true; }
        public HealthComponent(float maxHealth) { 
            CurrentHealth = MaxHealth = maxHealth;
            DirectiveUpdate = true;
        }

        protected override void ImplDirectiveSerialization()
        {
            base.ImplDirectiveSerialization();
            DirectiveUpdateContainer = new RawHealthComponentDirectUpdateEvent()
            {
                CurrentHealth = this.CurrentHealth,
                MaxHealth = this.MaxHealth,
                PlayerUpdateEntityId = this.ownerEntity.instanceId,
                ComponentInstanceId = this.instanceId
            };
        }


        public float CurrentHealth { get; set; }

        public float AwaitAcceptChanges { get; set; }
        public float MaxHealth { get; set; }
    }
}
