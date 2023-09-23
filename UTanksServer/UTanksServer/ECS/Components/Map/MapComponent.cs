using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Lobby;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1434697525077)]
    public class MapComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public MapValue map;
    }
}
