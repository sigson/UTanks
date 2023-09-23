using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types;

namespace UTanksClient.ECS.Components.Notification
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
