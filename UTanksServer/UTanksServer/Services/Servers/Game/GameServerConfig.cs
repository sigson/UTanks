using System.Net;

namespace UTanksServer.Services.Servers.Game {
  public class GameServerConfig {
    public IPAddress Address { get; set; } = null!;
    public IPAddress PublicAddress { get; set; } = null!;

    public int Port { get; set; }
  }
}
