using System;

using YamlDotNet.Serialization;

namespace UTanksServer.Services.Servers.Static.Models {
  public class StartupConfig {
    [YamlMember(typeof(string), Alias = "initUrl")] public Uri InitUri { get; set; } = null!;
    [YamlMember(typeof(string), Alias = "stateUri")] public Uri StateUri { get; set; } = null!;

    [YamlMember(Alias = "currentClientVersion")] public string ClientVersion { get; set; } = null!;
  }
}
