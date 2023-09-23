using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1479820450460)]
    public sealed class WebIdComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public string WebId { get; set; } = "";
        public string Utm { get; set; } = "";
        public string GoogleAnalyticsId { get; set; } = "";
        public string WebIdUid { get; set; } = "";
    }
}