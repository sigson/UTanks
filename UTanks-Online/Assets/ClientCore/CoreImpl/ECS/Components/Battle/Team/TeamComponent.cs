using System.Collections.Generic;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle;

namespace UTanksClient.ECS.Components.Battle.Team
{
    [TypeUid(-1462317917257855335)]
    public class TeamComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

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
            }
        }//flags in ctf, or frag in tdm, or all kills in dm
        public float MiddleRank => RankSum / (float)Players;
        public float RankSum { get; set; } = 0;
        public int Players { get; set; } = 0;

        //public List<ECSComponent> ComponentsForPlayerAppend = new List<ECSComponent>();//mainly aggregators components
        //public List<ECSComponent> DisabledCharacteristicTransformers = new List<ECSComponent>();
        //public List<ECSComponent> DisabledEffects = new List<ECSComponent>();
        //public List<ECSComponent> DisabledResistance = new List<ECSComponent>();
    }
}