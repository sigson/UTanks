using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.Converters
{
    public class IpAddressConverter : JsonConverter<IPAddress>
    {
        public override void WriteJson(JsonWriter writer, IPAddress? address, JsonSerializer serializer)
        {
            writer.WriteValue(address?.MapToIPv4().ToString());
        }

        public override IPAddress? ReadJson(
          JsonReader reader,
          Type objectType,
          IPAddress? existingValue,
          bool hasExistingValue,
          JsonSerializer serializer
        )
        {
            string? value = (string?)reader.Value;
            return value != null ? IPAddress.Parse(value).MapToIPv4() : null;
        }
    }
}
