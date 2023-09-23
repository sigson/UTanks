using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer
{
    public abstract class CachingSerializable
    {
        private string serializedObject;
        private Action<JsonWriter, JsonSerializer> GetSerializedObject;

        public CachingSerializable()
        {
            GetSerializedObject = SerializeInit;
        }

        public void Serialize(JsonWriter writer, JsonSerializer serializer)
        {
            GetSerializedObject(writer, serializer);
        }

        private void SerializeInit(JsonWriter writer, JsonSerializer serializer)
        {
            if (serializedObject == null)
            {
                using (StringWriter stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, this);
                    serializedObject = stringWriter.ToString();
                }
            }

            GetSerializedObject = (wr, sr) =>
            {
                wr.WriteRawValue(serializedObject);
            };

            GetSerializedObject(writer, serializer);
        }
    }

    public class CachingConverter : JsonConverter
    {
        private JsonSerializer defaultSerializer;

        public CachingConverter(JsonSerializer serializer)
        {
            this.defaultSerializer = serializer;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var cacheValue = (CachingSerializable)value;
            cacheValue.Serialize(writer, this.defaultSerializer);
        }
    }

    public class CachingContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            if (typeof(CachingSerializable).IsAssignableFrom(objectType))
            {
                //Create default serializer with default contract resolver and converters
                var serializer = JsonSerializer.Create(CachingSettings.Default);
                //Add custom converter to the serializer
                contract.Converter = new CachingConverter(serializer);
            }

            return contract;
        }
    }

    public static class CachingSettings
    {
        public static JsonSerializerSettings Default { get; private set; }

        static CachingSettings()
        {
            Default = new JsonSerializerSettings();
            Default.Converters.Add(new StringEnumConverter());
        }

    }

    public static class GlobalCachingSerialization
    {
        public static JsonSerializer cachingSerializer = JsonSerializer.Create(CachingSettings.Default);
        public static JsonSerializer standartSerializer = new JsonSerializer();
        public static void Init()
        {
            cachingSerializer.ContractResolver = new CachingContractResolver();
        }
    }
}
