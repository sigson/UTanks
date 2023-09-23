using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Time
{
    [TypeUid(92197374614905239)]
    public class RoundStopTimeComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public RoundStopTimeComponent() { }
        public RoundStopTimeComponent(DateTimeOffset stopTime)
        {
            StopTime = stopTime.ToUnixTimeMilliseconds();
        }

        public long StopTime { get; set; }
    }
}