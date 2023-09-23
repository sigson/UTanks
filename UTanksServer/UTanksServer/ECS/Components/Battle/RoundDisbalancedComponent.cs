using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types;
using UTanksServer.ECS.Types.Battle.Team;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(3051892485776042754L)]
    public class RoundDisbalancedComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public RoundDisbalancedComponent() { }
        public RoundDisbalancedComponent(TeamComponent Loser, int InitialDominationTimerSec, DateTime FinishTime)
        {
            this.Loser = Loser;
            this.InitialDominationTimerSec = InitialDominationTimerSec;
            this.FinishTime = FinishTime;
        }

        public TeamComponent Loser { get; set; }
        public int InitialDominationTimerSec { get; set; }
        public DateTime FinishTime { get; set; }
    }
}