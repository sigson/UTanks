using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle;

namespace UTanksServer.ECS.Components.Battle.Team
{
    [TypeUid(-1462317917257855335)]
    public class TeamComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        [field: NonSerialized]
        [JsonIgnore]
        public GroupDataAccessPolicy teamGDAP { get; set; }
        public string TeamColor { get; set; }
        public long BattleEntityInstanceId;
        public int FinalGoalValue { get; set; }
        private int goalScore = 0;
        public int GoalScore { 
            get 
            {
                return goalScore;
            }
            set
            {
                goalScore = value;
                if(goalScore >= FinalGoalValue && FinalGoalValue != 0)
                {
                    ManagerScope.eventManager.OnEventAdd(new BattleEndEvent() { TeamWinnerInstanceId = this.instanceId, BattleEntity = this.BattleEntityInstanceId });
                }
            }
        }//flags in ctf, or frag in tdm, or all kills in dm
        [JsonIgnore]
        public float MiddleRank => RankSum / (float)Players;
        public float RankSum { get; set; } = 0;
        public int Players { get; set; } = 0;

        [NonSerialized]
        [JsonIgnore]
        public List<ECSComponent> ComponentsForPlayerAppend = new List<ECSComponent>();//mainly aggregators components
        [NonSerialized]
        [JsonIgnore]
        public List<ECSComponent> DisabledCharacteristicTransformers = new List<ECSComponent>();
        [NonSerialized]
        [JsonIgnore]
        public List<ECSComponent> DisabledEffects = new List<ECSComponent>();
        [NonSerialized]
        [JsonIgnore]
        public List<ECSComponent> DisabledResistance = new List<ECSComponent>();
    }
}