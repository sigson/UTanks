using System.Collections.Generic;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
	[TypeUid(1499174753575)]
	public class UserStatisticsComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public Dictionary<string, long> Statistics { get; set; } = new Dictionary<string, long>()
		{
			{ "HEALED", 0 },
			{ "CTF_CARRIAGE_SCORE", 0 },
			{ "SCORE", 0 },
			{ "ENERGY_COMPENSATION", 0 },
			{ "BATTLES_PARTICIPATED", 0 },
			{ "DEATHS", 0 },
			{ "CTF_RETURN_SCORE", 0 },
			{ "SUICIDES", 0 },
			{ "ALL_CUSTOM_BATTLES_PARTICIPATED", 0 },
			{ "HEAL_XP", 0 },
			{ "KILL_SCORE", 0 },
			{ "VICTORIES", 0 },
			{ "CTF_PLAYED", 0 },
			{ "DEFEATS", 0 },
			{ "DM_PLAYED", 0 },
			{ "HITS", 0 },
			{ "KILL_ASSIST_XP", 0 },
			{ "KILL_XP", 0 },
			{ "CURRENT_WINNING_STREAK", 0 },
			{ "ENERGY", 0 },
			{ "CTF_CARRIAGE_XP", 0 },
			{ "CTF_RETURN_XP", 0 },
			{ "SHOTS", 0 },
			{ "DRAWS", 0 },
			{ "ALL_BATTLES_PARTICIPATED", 0 },
			{ "PUNISHMENT_SCORE", 0 },
			{ "KILLS", 0 },
			{ "HEAL_SCORE", 0 },
			{ "TDM_PLAYED", 0 },
			{ "XP", 0 },
			{ "KILL_ASSIST_SCORE", 0 },
			{ "BATTLES_PARTICIPATED_IN_SEASON", 0 },
			{ "CUSTOM_BATTLES_PARTICIPATED", 0 },
			{ "GOLDS", 0 },
		};
	}
}
