using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "Subscription", Namespace = XmlNamespaces.Default)]
    public class Subscription
    {
        [XmlElement(ElementName = "id", Order = 0)]
        public Identity Id { get; set; }

        [XmlElement(ElementName = "link", Order = 1)]
        public Link Target { get; set; }

        [XmlElement(ElementName = "filter", Order = 2)]
        public string Filter { get; set; }
        
        [XmlElement(ElementName = "callback", Order = 3)]
        public Callback Callback { get; set; }
    }
}