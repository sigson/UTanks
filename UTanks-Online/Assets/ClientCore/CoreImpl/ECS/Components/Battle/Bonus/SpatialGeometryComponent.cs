using System.Numerics;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Bonus
{
    [TypeUid(8960819779144518L)]
    public class SpatialGeometryComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SpatialGeometryComponent() { }
        public SpatialGeometryComponent(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }
}