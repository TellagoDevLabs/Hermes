using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName="topic", Namespace = XmlNamespaces.Default)]
    public class TopicPost
    {
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "groupId")]
        public Identity GroupId { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }
}
