using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Time
{
    [TypeUid(-3596341255560623830)]
    public class TimeLimitComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TimeLimitComponent() { }
        public TimeLimitComponent(long timeLimitSec, long warmingUpTimeLimitSet)
        {
            TimeLimitSec = timeLimitSec;
            WarmingUpTimeLimitSet = warmingUpTimeLimitSet;
        }
        
        public long TimeLimitSec { get; set; }
        
        public long WarmingUpTimeLimitSet { get; set; }
    }
}