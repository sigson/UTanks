using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Round
{
    [TypeUid(6921712768819133913)]
    public class RoundUserStatisticsComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public RoundUserStatisticsComponent(int place, int scoreWithoutBonuses, int kills, int killAssists, int deaths, string nickname, int rank)
        {
            Place = place;
            ScoreWithoutBonuses = scoreWithoutBonuses;
            Kills = kills;
            KillAssists = killAssists;
            Deaths = deaths;
            Nickname = nickname;
            Rank = rank;
        }

        public RoundUserStatisticsComponent() => Place = ScoreWithoutBonuses = Kills = KillAssists = Deaths = 0;

        public int Place { get; set; }

        public int ScoreWithoutBonuses { get; set; }

        public int Kills { get; set; }

        public int KillAssists { get; set; }

        public int Deaths { get; set; }

        public string Nickname { get; set; }

        public int Rank { get; set; }
    }
}
