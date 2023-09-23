using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Time
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