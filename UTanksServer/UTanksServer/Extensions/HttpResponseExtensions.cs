using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

using EmbedIO;

namespace UTanksServer.Extensions {
  public static class HttpResponseExtensions {
    public static async Task Send(this IHttpResponse response, Stream stream, string? contentType = null, bool rewind = true) {
      if(contentType != null) response.ContentType = contentType;
      if(rewind) stream.Rewind();

      await stream.CopyToAsync(response.OutputStream);
    }

    public static async Task SendPlain(this IHttpResponse response, Stream stream, bool rewind = true) =>
      await response.Send(stream, MediaTypeNames.Text.Plain, rewind);

    public static async Task SendJson(this IHttpResponse response, Stream stream, bool rewind = true) =>
      await response.Send(stream, MediaTypeNames.Application.Json, rewind);

    public static async Task SendBinary(this IHttpResponse response, Stream stream, bool rewind = true) =>
      await response.Send(stream, MediaTypeNames.Application.Octet, rewind);
  }
}
