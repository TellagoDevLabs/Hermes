using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "topic", Namespace = XmlNamespaces.Default)]
    public class TopicPut
    {
        [XmlElement(ElementName = "id")]
        public Identity Id { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "groupId")]
        public Identity GroupId { get; set; }
    }
}