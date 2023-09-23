using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(-5407563795844501148L)]
    public class StreamHitConfigComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public float LocalCheckPeriod { get; set; }
        public float SendToServerPeriod { get; set; }
        public bool DetectStaticHit { get; set; }
    }
}
