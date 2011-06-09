using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot("link", Namespace = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade")]
    public class Link
    {
        [XmlAttribute("rel")]
        public string Rel { get; set; }

        [XmlIgnore]
        public Uri  HRef { get; set; }

        [XmlAttribute("href")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string HRefString
        {
            get { return HRef == null ? null : HRef.ToString(); }
            set { HRef = value == null ? null : new Uri(value); }
        }

    }
}
