using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "Subscription", Namespace = XmlNamespaces.Default)]
    public class SubscriptionPost
    {
        [XmlElement(ElementName = "topicId", Order = 0)]
        public Identity? TopicId { get; set; }

        [XmlElement(ElementName = "groupId", Order = 1)]
        public Identity? GroupId { get; set; }

        [XmlElement(ElementName = "callback", Order = 2)]
        public Callback Callback { get; set; }
    }
}