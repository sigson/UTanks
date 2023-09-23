using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Notification
{
    [TypeUid(1493197354957L)]
    public class ServerNotificationMessageComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ServerNotificationMessageComponent() { }
        public ServerNotificationMessageComponent(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
