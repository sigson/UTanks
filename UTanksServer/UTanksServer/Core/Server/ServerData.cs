using System;
using System.Collections.Generic;
using UTanksServer.Services.Servers.Game;
//using UTanksServer.ECSSystem.GlobalEntities;

namespace UTanksServer.Core
{
    public class ServerData
    {
        public ServerData()
        {
            // todo: load data from database
        }

        public bool FractionsCompetitionActive { get; set; }
        public bool FractionsCompetitionFinished { get; set; }
        public long FractionsCompetitionCryFund => (AntaeusScore + FrontierScore) * 3;
        public long AntaeusScore
        {
            get => _antaeusScore;
            set
            {
                _antaeusScore = value;
                //foreach (Player player in Server.Instance.Connection.Pool)
                //    player.UpdateFractionScores();
            }
        }
        public long AntaeusUserCount { get; set; }
        public long FrontierScore
        {
            get => _frontierScore;
            set
            {
                _frontierScore = value;
                //foreach (Player player in Server.Instance.Connection.Pool)
                //    player.UpdateFractionScores();
            }
        }
        public long FrontierUserCount { get; set; }

        public bool SpreadLastSeasonRewards { get; set; }
        public int SeasonNumber { get; set; }

        public bool SpreadReleaseGift { get; set; }
        public DateTimeOffset ReleaseGiftMaxRegistrationDate { get; set; } = DateTimeOffset.UtcNow;


        private long _antaeusScore;
        private long _frontierScore;

    }
}
