using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Chassis
{
    [TypeUid(1437725485852)]
    public class DampingComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public DampingComponent() { }
        public DampingComponent(float damping)
        {
            Damping = damping;
        }

        public float Damping { get; set; }
    }
}