using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "Subscription", Namespace = XmlNamespaces.Default)]
    public class SubscriptionPut
    {
        [XmlElement(ElementName = "id", Order = 0)]
        public Identity Id { get; set; }

        [XmlElement(ElementName = "callback", Order = 1)]
        public Callback Callback { get; set; }
    }
}