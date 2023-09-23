using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(2930474294118078222)]
    public class IdleCounterComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public IdleCounterComponent() { }
        public IdleCounterComponent(long skippedMillis, DateTime? skipBeginDate = null)
        {
            SkipBeginDate = skipBeginDate;
            SkippedMillis = skippedMillis;
        }
        
        [OptionalMapped]
        public DateTime? SkipBeginDate { get; set; }
        
        public long SkippedMillis { get; set; }
    }
}