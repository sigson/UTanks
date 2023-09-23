using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
	[TypeUid(-777019732837383198)]
	public class UserScoreComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public UserScoreComponent() { }
		public UserScoreComponent(int globalScore, int rankScore, int nextRankScoreThreshold)
		{
			this.RankScore = rankScore;
			this.GlobalScore = globalScore;
			this.NextRankScoreThreshold = nextRankScoreThreshold;
		}

		public int GlobalScore { get; set; }
		public int RankScore { get; set; }
		public int NextRankScoreThreshold { get; set; }
	}
}
