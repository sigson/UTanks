using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Health
{
    [TypeUid(8420700272384380156)]
    public class HealthConfigComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public HealthConfigComponent() { }
        public HealthConfigComponent(float baseHealth)
        {
            BaseHealth = baseHealth;
        }
        
        public float BaseHealth { get; set; }
    }
}