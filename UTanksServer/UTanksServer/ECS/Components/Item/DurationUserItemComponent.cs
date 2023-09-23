using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1513581047619L)]
    public class DurationUserItemComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddHours(6);
    }
}
