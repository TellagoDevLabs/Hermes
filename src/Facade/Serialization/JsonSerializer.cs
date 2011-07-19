using System;
using System.IO;
using Newtonsoft.Json;

namespace TellagoStudios.Hermes.Facade.Serialization
{
    public static class JsonSerializer
    {
        class IdentityJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Identity);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var str = reader.Value.ToString();
                return new Identity(str);
            }

            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                var str = value.ToString();
                writer.WriteValue(str);
            }
        }

        static private IdentityJsonConverter identityJsonConverter = new IdentityJsonConverter();

        static public void Serialize(object value, Stream stream)
        {
            var writer = new StreamWriter(stream);
            writer.Write(JsonConvert.SerializeObject(value, identityJsonConverter));
            writer.Flush();
        }

        static public T Deserialize<T>(Stream stream)
        {
            var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var value = JsonConvert.DeserializeObject<T>(json, identityJsonConverter);
            return value;
        }

        static public object Deserialize(Type type, Stream stream)
        {
            var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var value = JsonConvert.DeserializeObject(json, type, identityJsonConverter);
            return value;
        }
    }
}
