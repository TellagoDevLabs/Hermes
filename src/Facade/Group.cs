using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "Group", Namespace = XmlNamespaces.Default)]
    public class Group
    {
        [XmlElement(ElementName = "id", Order = 0)]
        public Identity Id { get; set; }

        [XmlElement(ElementName = "name", Order = 1)]
        public string Name { get; set; }

        [XmlElement(ElementName = "description", Order = 2)]
        public string Description { get; set; }

        [XmlElement(Order = 3, ElementName = "link")]
        public Link Parent { get; set; }
    }
}