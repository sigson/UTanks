using System;

using Serilog;
using Serilog.Core;

using UTanksServer.Services.Servers.Game;

namespace UTanksServer.Extensions {
  public static class SerilogExtensions {
    public static ILogger ForType<T>(this ILogger logger) => logger.ForContext(Constants.SourceContextPropertyName, typeof(T).Name);
    public static ILogger ForType(this ILogger logger, Type type) => logger.ForContext(Constants.SourceContextPropertyName, type.Name);

    public static ILogger WithPlayer(this ILogger logger, Player player) => logger
      .ForContext("Player", player)
      .ForContext("PlayerLogDisplay", player.LogDisplay);
  }
}
