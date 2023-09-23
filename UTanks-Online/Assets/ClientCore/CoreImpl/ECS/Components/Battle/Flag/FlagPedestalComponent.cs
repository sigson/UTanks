using System.Numerics;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(4898317045808451550L)]
    public class FlagPedestalComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public FlagPedestalComponent() { }
        public FlagPedestalComponent(Vector3S position)
        {
            Position = position;
        }

        public Vector3S Position { get; set; }
    }
}