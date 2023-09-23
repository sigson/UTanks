using System.Collections.Generic;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(636174034381039231)]
    public class ItemsPackFromConfigComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public List<long> Pack { get; set; } = new List<long>();
    }
}
