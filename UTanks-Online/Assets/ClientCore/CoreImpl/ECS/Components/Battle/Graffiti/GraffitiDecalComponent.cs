using System.Numerics;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle
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