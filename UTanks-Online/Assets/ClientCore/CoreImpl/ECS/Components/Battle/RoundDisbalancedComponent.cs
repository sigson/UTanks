using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.Components.Battle.Team;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types;
using UTanksClient.ECS.Types.Battle.Team;

namespace UTanksClient.ECS.Components.Battle
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