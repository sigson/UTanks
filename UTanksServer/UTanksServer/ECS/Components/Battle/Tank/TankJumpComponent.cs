using System.Numerics;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(1835748384321L)]
    public class TankJumpComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public float StartTime { get; set; }
        public Vector3 Velocity { get; set; }
        public bool OnFly { get; set; }
        public bool Slowdown { get; set; }
        public float SlowdownStartTime { get; set; }
    }
}