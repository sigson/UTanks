using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Team
{
    [TypeUid(-2440064891528955383)]
    public class TeamScoreComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TeamScoreComponent() => Score = 0;

        public int Score { get; set; }
    }
}
