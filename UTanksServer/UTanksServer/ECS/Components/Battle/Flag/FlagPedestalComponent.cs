using System.Numerics;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle;

namespace UTanksServer.ECS.Components.Battle
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