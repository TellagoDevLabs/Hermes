using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot("link", Namespace = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade")]
    public class Link
    {
        [XmlAttribute]
        public string rel { get; set; }

        [XmlAttribute]
        public string href { get; set; }
    }
}
