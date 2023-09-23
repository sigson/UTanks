using System.IO;
using System.Text.Json;
using UTanksServer.Core.Database;
using UTanksServer.Database;

namespace UTanksServer.Core
{
    public struct ServerConfig
    {
        public static ServerConfig Load(string path) {
            Instance = JsonSerializer.Deserialize<ServerConfig>(File.ReadAllText(GlobalProgramState.ConfigDir + path), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return Instance;
        }

        public static ServerConfig Instance { get; private set; }
        public DatabaseNetworkConfig DatabaseNetwork { get; set; }
    }
}
