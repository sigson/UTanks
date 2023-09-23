using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(1437989437781L)]
    public class KickbackComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public KickbackComponent() { }
        public KickbackComponent(float kickbackForce)
        {
            KickbackForce = kickbackForce;
        }

        public float KickbackForce { get; set; }
    }
}