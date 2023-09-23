using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(1437983636148L)]
    public class ImpactComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ImpactComponent() { }
        public ImpactComponent(float impactForce)
        {
            ImpactForce = impactForce;
        }

        public float ImpactForce { get; set; }
    }
}