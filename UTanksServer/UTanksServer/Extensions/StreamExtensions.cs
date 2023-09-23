using System.IO;

namespace UTanksServer.Extensions {
  public static class StreamExtensions {
    public static void Rewind(this Stream stream) => stream.Position = 0;
  }
}
