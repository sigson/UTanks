using YamlDotNet.Serialization;

namespace UTanksServer.Services.Servers.Static.Models {
  public class StateConfig {
    [YamlMember(Alias = "state")] public int State { get; set; }
  }
}
