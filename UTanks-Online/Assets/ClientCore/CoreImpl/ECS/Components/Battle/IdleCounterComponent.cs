using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle
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