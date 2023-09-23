using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using Serilog;

using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;

using UTanksServer.Extensions;

namespace UTanksServer.Services {
  public interface IClientConfigService {
    public Task Init();
    public MemoryStream GetArchiveStream(string version);
  }

  [UTanksServer.ECS.ECSCore.Service]
  public class ClientConfigService : IClientConfigService {
    private static readonly ILogger Logger = Log.Logger.ForType<ClientConfigService>();

    private readonly Dictionary<string, byte[]> _versions;

    public ClientConfigService() {
      _versions = new Dictionary<string, byte[]>();
    }

    public async Task Init() {
      Logger.Information("Generating config archives...");

      DirectoryInfo rootDirectory = new DirectoryInfo("Static/Config");
      foreach(DirectoryInfo directory in rootDirectory.EnumerateDirectories()) {
        string version = directory.Name;

        Logger.Debug("Generating config archive for version {Version}...", version);

        Stopwatch stopwatch = new Stopwatch();

        await using MemoryStream archiveStream = new MemoryStream();
        await using(GZipOutputStream gzip = new GZipOutputStream(archiveStream)) {
          await using TarOutputStream tar = new TarOutputStream(gzip, Encoding.UTF8);

          stopwatch.Start();
          foreach(FileInfo file in directory.EnumerateFiles("*.*", SearchOption.AllDirectories)) {
            string path = Path.GetRelativePath(directory.FullName, file.FullName).TrimStart('/');

            MemoryStream fileStream = await file.ReadAsync();
            TarEntry entry = TarEntry.CreateTarEntry(path);

            entry.Size = fileStream.Length;

            tar.PutNextEntry(entry);
            await fileStream.CopyToAsync(tar);

            tar.CloseEntry();
          }
          stopwatch.Stop();
        }

        _versions.Add(version, archiveStream.ToArray());

        Logger.Debug(
          "Generated config archive for version {Version} in {Duration} ms",
          version,
          stopwatch.ElapsedMilliseconds
        );
      }

      Logger.Information("Initialized");
    }

    public MemoryStream GetArchiveStream(string version) {
      if(!_versions.TryGetValue(version, out byte[]? buffer)) {
        throw new InvalidOperationException($"Config archive for version {version} is not initialized");
      }

      return new MemoryStream(buffer);
    }
  }
}
