using Newtonsoft.Json;

namespace UTanksServer.Database {
  public class DatabaseConfig {
    [JsonProperty("host")] public string Host { get; set; } = null!;
    [JsonProperty("port")] public int Port { get; set; }
    [JsonProperty("username")] public string Username { get; set; } = null!;
    [JsonProperty("password")] public string Password { get; set; } = null!;
    [JsonProperty("database")] public string Database { get; set; } = null!;
    [JsonProperty("version")] public string Version { get; set; } = null!;
  }
}
