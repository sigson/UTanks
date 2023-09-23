using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Lobby;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1434697525077)]
    public class MapComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public MapValue map;
    }
}
