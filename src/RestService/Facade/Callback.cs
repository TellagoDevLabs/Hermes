using System.Xml.Serialization;

namespace TellagoStudios.Hermes.RestService.Facade
{
    [XmlRoot(ElementName = "callback", Namespace = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade")]
    public class Callback
    {
        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("kind")]
        public CallbackKind Kind { get; set; }
    }
}