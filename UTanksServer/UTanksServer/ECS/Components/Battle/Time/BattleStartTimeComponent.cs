using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Time
{
    [TypeUid(1436521738148L)]
    public class BattleStartTimeComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BattleStartTimeComponent() { }
        public BattleStartTimeComponent(DateTimeOffset roundStartTime)
        {
            RoundStartTime = roundStartTime.ToUnixTimeMilliseconds();
        }

        [OptionalMapped] public long RoundStartTime { get; set; }
    }
}