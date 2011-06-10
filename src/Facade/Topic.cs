using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "Topic", Namespace = XmlNamespaces.Default)]
    public class Topic
    {
        [XmlElement(ElementName="id", Order=0)]
        public Identity Id { get; set; }

        [XmlElement(ElementName = "name", Order = 1)]
        public string Name { get; set; }

        [XmlElement(ElementName = "description", Order = 2)]
        public string Description { get; set; }

        [XmlElement(ElementName="link", Order=3)]
        public Link Group { get; set; }
    }
}
