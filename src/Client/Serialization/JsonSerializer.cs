using System.IO;

namespace TellagoStudios.Hermes.Client.Serialization
{
    class JsonSerializer : ISerializer
    {
        public void Serialize<T>(Stream stream, T data)
        {
            Facade.Serialization.JsonSerializer.Serialize(data, stream);
        }

        public T Deserialize<T>(Stream stream)
        {
            return Facade.Serialization.JsonSerializer.Deserialize<T>(stream);
        }
    }
}
