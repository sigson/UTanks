using System;

using YamlDotNet.Serialization;

namespace UTanksServer.Services.Servers.Static.Models {
  public class InitConfig {
    [YamlMember(Alias = "configVersion")] public string ConfigVersion { get; set; } = null!;
    [YamlMember(Alias = "bundleDbVersion")] public string BundleDbVersion { get; set; } = null!;
    [YamlMember(Alias = "host")] public string Host { get; set; } = null!;
    [YamlMember(Alias = "acceptorPort")] public int Port { get; set; }

    [YamlMember(typeof(string), Alias = "configsUrl")] public Uri ConfigsUri { get; set; } = null!;

    /// <remarks>This property is not used by the client</remarks>
    [YamlMember(typeof(string), Alias = "stateFileUrl")] public Uri StateFileUri { get; set; } = null!;

    [YamlMember(typeof(string), Alias = "resourcesUrl")] public Uri ResourcesUri { get; set; } = null!;
    [YamlMember(typeof(string), Alias = "updateConfigUrl")] public Uri UpdateConfigUri { get; set; } = null!;
  }
}
