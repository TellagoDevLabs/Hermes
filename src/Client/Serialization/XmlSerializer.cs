using System;
using System.IO;
using System.Xml;
using Xml=System.Xml.Serialization;

namespace TellagoStudios.Hermes.Client.Serialization
{
    class XmlSerializer : ISerializer
    {
        static private readonly string HermesNameSpace = typeof(Facade.Group).Namespace;

        public void Serialize<T>(Stream stream, T data)
        {
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var ns = new Xml.XmlSerializerNamespaces();

                if (typeof(T).Namespace == HermesNameSpace)
                {
                    ns.Add("", Facade.XmlNamespaces.Default);
                }

                var serializer = new Xml.XmlSerializer(typeof(T));
                serializer.Serialize(writer, data, ns);

                writer.Close();
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            var serializer = new Xml.XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }
    }
}
