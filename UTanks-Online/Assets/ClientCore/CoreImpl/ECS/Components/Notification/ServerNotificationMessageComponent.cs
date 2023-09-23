using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Notification
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
