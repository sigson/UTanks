using System.Net;

namespace UTanksServer.Services.Servers.Static {
  public class StaticServerConfig {
    public IPAddress Address { get; set; } = null!;
    public IPAddress PublicAddress { get; set; } = null!;

    public int Port { get; set; }
  }
}
