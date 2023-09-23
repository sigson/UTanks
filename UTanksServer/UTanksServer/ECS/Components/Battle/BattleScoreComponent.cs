using System.Collections.Concurrent;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Round;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(1436532217083L)]
    public class BattleScoreComponent : ECSComponent //obsolete
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public ConcurrentDictionary<long, Command> TeamsScore
        {
            get 
            {
                if(teamsScore.Count == 0)
                {
                    ConcurrentDictionary<long, Command> localTeamsScore = new ConcurrentDictionary<long, Command>();
                    ownerEntity.GetComponent<BattleTeamsComponent>().teams.ForEach(x =>
                    {
                        localTeamsScore.TryAdd(x.Key, new Command {
                            GoalScore = x.Value.GoalScore,
                            TeamColor = x.Value.TeamColor
                        });
                    });
                    return localTeamsScore;
                }
                else
                {
                    return teamsScore;
                }
            }
            set 
            {
                teamsScore = value;
            }
        }
        private ConcurrentDictionary<long, Command> teamsScore = new ConcurrentDictionary<long, Command>();
    }
}
