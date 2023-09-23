using System.Collections.Generic;

namespace UTanksClient.ECS.Types.Battle.Team {
    public static class TeamColors
    {
        public static Dictionary<string, long> colors = new Dictionary<string, long>()
        {
            {"dm", 0x57d927 },
            {"red", 0xFF3300 },
            {"blue", 0x0072FF },
            {"violet", 0xab00ad },
            {"yellow", 0xdae600 },
            {"orange", 0xe07400 },
            {"lightblue", 0x00d1c3 },
            {"pink", 0xe000d1 },
            {"disabled", 0x1d1a1a },
        };

        public static Dictionary<string, (long, float)> elementColorOffsets = new Dictionary<string, (long, float)>()
        {
            {"battlePlayerRow", (0xFFFFFF, 0.2f) },
            {"battlePlayerRowsBackground", (0x000000, 0.4f) },
            {"battlePlayerRowsHeader", (0xFFFFFF, 0.5f) },
            {"nicknameColor", (0x000000, 0.04f) },
            {"healthBackground", (0x000000, 0.5f) },
            {"teamStatsColor", (0xFFFFFF, 0.25f) },
        };
    }
}
