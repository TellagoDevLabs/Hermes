using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName="topic", Namespace = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade")]
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
