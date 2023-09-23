using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(-3048295118496552479)]
    public class ScoreLimitComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ScoreLimitComponent()
        {
            
        }
        public ScoreLimitComponent(int scoreLimit)
        {
            ScoreLimit = scoreLimit;
        }
        
        public int ScoreLimit { get; set; }
    }
}