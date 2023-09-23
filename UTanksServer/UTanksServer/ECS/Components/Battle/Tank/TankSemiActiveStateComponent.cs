using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Tank
{
    [TypeUid(5166099393636831290)]
    public class TankSemiActiveStateComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TankSemiActiveStateComponent()
        {
            ActivationTime = .25f;
        }

        public TankSemiActiveStateComponent(float activationTime)
        {
            ActivationTime = activationTime;
        }

        public float ActivationTime { get; set; }
    }
}