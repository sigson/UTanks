using System.IO;
using System.Threading.Tasks;

namespace UTanksServer.Extensions {
  public static class FileInfoExtensions {
    public static async Task CopyToAsync(this FileInfo source, FileInfo destination) {
      await using FileStream sourceStream = source.CreateStream(FileMode.Open, FileAccess.Read, FileShare.Read);
      await using FileStream destinationStream = destination.CreateStream(FileMode.Create, FileAccess.Write, FileShare.None);

      await sourceStream.CopyToAsync(destinationStream);
    }

    public static async Task<MemoryStream> ReadAsync(this FileInfo file) {
      await using FileStream stream = file.CreateStream(FileMode.Open, FileAccess.Read, FileShare.Read);
      MemoryStream memory = new MemoryStream();

      await stream.CopyToAsync(memory);
      memory.Rewind();

      return memory;
    }

    public static async Task WriteAsync(this FileInfo file, MemoryStream stream) {
      await using FileStream fileStream = file.CreateStream(FileMode.Create, FileAccess.Write);
      await stream.CopyToAsync(fileStream);
    }

    public static FileStream CreateStream(
      this FileInfo file,
      FileMode mode,
      FileAccess access,
      FileShare share = FileShare.ReadWrite,
      int bufferSize = 4096,
      FileOptions options = FileOptions.Asynchronous | FileOptions.SequentialScan
    ) => new FileStream(file.FullName, mode, access, share, bufferSize, options);
  }
}
