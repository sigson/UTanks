using System.IO;

namespace UTanksClient.Extensions {
  public static class StreamExtensions {
    public static void Rewind(this Stream stream) => stream.Position = 0;
  }
}
