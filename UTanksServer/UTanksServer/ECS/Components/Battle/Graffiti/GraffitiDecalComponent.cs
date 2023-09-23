using System.Numerics;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(636100801609006236L)]
    public class GraffitiDecalComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public Vector3 SprayPosition { get; set; }
        public Vector3 SprayDirection { get; set; }
        public Vector3 SprayUpDirection { get; set; }
    }
}