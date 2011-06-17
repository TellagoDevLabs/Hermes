using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "callback", Namespace = XmlNamespaces.Default)]
    public class Callback
    {
        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("kind")]
        public CallbackKind Kind { get; set; }
    }
}