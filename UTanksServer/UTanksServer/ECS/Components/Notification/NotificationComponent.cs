using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types;

namespace UTanksServer.ECS.Components.Notification
{
    [TypeUid(1464339267328L)]
    public class NotificationComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //public NotificationComponent(NotificationPriority priority)
        //{
        //    Priority = priority;
        //}

        //public NotificationPriority Priority { get; set; }

        public DateTime TimeCreation { get; set; } = DateTime.UtcNow;
    }
}
