using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace UTanksServer.Database {
  public static class ModelBuilderExtensions {
    public static void OverrideConverter<T>(this ModelBuilder builder, ValueConverter converter) {
      foreach(IMutableProperty property in builder.Model.GetEntityTypes().SelectMany((type) => type.GetProperties())) {
        if(property.ClrType == typeof(T) || property.ClrType == typeof(T?)) {
          property.SetValueConverter(converter);
        }
      }
    }
  }
}
