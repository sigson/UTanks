using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Chassis
{
    [TypeUid(1437571863912)]
    public class WeightComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WeightComponent() { }
        public WeightComponent(float weight)
        {
            Weight = weight;
        }

        public float Weight { get; set; }
    }
}