using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Health
{
    [TypeUid(1949198098578360952)]
    public class HealthComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public HealthComponent() { }
        public HealthComponent(float maxHealth) => CurrentHealth = MaxHealth = maxHealth;

        public float CurrentHealth { get; set; }

        public float AwaitAcceptChanges { get; set; }
        public float MaxHealth { get; set; }
    }
}
