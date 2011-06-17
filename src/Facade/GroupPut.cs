using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "Group", Namespace = XmlNamespaces.Default)]
    public class GroupPut
    {
        [XmlElement(ElementName = "id", Order = 0)]
        public Identity Id { get; set; }

        [XmlElement(ElementName = "name", Order = 1)]
        public string Name { get; set; }

        [XmlElement(ElementName = "description", Order = 2)]
        public string Description { get; set; }

        [XmlElement(ElementName = "parentId", Order = 3)]
        public Identity? ParentId { get; set; }
    }
}