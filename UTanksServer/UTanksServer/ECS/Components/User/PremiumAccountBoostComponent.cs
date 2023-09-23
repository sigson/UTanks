using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1513252416040L)]
    public class PremiumAccountBoostComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public PremiumAccountBoostComponent() { }
        public PremiumAccountBoostComponent(DateTime endDate) => EndDate = endDate;

        public DateTime EndDate { get; set; }
    }
}
