using System.Collections.Generic;

namespace UTanksServer.ECS.Types.Battle.Team {
    public static class TeamColors
    {
        public static Dictionary<string, long> colors = new Dictionary<string, long>()
        {
            {"dm", 0x0ba100 },
            {"red", 0xd10000 },
            {"blue", 0x0c00b8 },
            {"violet", 0xab00ad },
            {"yellow", 0xdae600 },
            {"orange", 0xe07400 },
            {"lightblue", 0x00d1c3 },
            {"pink", 0xe000d1 },
        };
    }
}
