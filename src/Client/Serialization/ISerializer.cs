using System.IO;

namespace TellagoStudios.Hermes.Client.Serialization
{
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T data);
        T Deserialize<T>(Stream stream);
    }
}
