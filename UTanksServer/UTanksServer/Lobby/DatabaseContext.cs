using System;
using System.Linq;
using System.Net;

using Serilog;

using MySqlConnector;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using UTanksServer.Extensions;
using UTanksServer.Services.Servers.Game;

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UTanksServer.Database {
  public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext> {
    public DatabaseContext CreateDbContext(string[] args) {
      return new DatabaseContext(new DatabaseConfig() {
        Host = "127.0.0.1",
        Port = 3306,
        Username = "araumi",
        Password = "araumi",
        Database = "araumi",
        Version = "10.6.4"
      });
    }
  }


  public class DatabaseContext : DbContext {
    private static readonly ILogger Logger = Log.Logger.ForType<DatabaseContext>();

    private readonly DatabaseConfig _config;

    public DbSet<PlayerData> Players { get; protected set; } = null!;

    public DatabaseContext(DatabaseConfig config) {
      _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder) {
      // builder.UseLoggerFactory(LoggerFactory.Create(builder => builder
      //   .AddFilter((category, level) => true)
      //   .AddConsole()
      // ));
      builder.EnableSensitiveDataLogging();

      MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder() {
        Server = _config.Host,
        Port = (uint)_config.Port,
        UserID = _config.Username,
        Password = _config.Password,
        Database = _config.Database
      };

      Logger.Information(
        "Connecting to mysql://{Username}@{Host}:{Port}/{Database}...",
        _config.Username,
        _config.Host,
        _config.Port,
        _config.Database
      );

      builder.UseMySql(
        new MySqlConnection(stringBuilder.ToString()),
        new MariaDbServerVersion(Version.Parse(_config.Version))
      );
    }

    protected override void OnModelCreating(ModelBuilder builder) {
      // ValueConverter<Entity, long> entityConverter = new ValueConverter<Entity, long>(
      //   (entity) => entity.Id,
      //   (id) => ...
      // );

      builder.OverrideConverter<IPAddress>(new IPAddressToBytesConverter());
      builder.OverrideConverter<TimeSpan>(new TimeSpanToTicksConverter());

      builder.Entity<PlayerData>((entity) => {
        entity.HasKey((player) => new { player.Id });
      });

      Logger.Debug("Database models:");
      foreach(IMutableEntityType type in builder.Model.GetEntityTypes()) {
        Logger.Debug(
          "  - {Name} (primary keys: {Keys}, foreign keys: {ForeignKeys}, properties: {Properties})",
          type.ClrType.Name,
          type.GetKeys().Count(),
          type.GetForeignKeys().Count(),
          type.GetProperties().Count()
        );
      }
    }
  }
}
