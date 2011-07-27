using System;
using System.IO;
using System.Text;

namespace TellagoStudios.Hermes.Client.Serialization
{
    class TextSerializer : ISerializer
    {
        public void Serialize<T>(Stream stream, T data)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(Convert.ToString(data));
                writer.Flush();
                writer.Close();
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var data = reader.ReadToEnd();
                return (T) Convert.ChangeType(data, typeof (T));
            }
        }
    }
}
