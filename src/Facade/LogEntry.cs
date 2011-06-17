using System;
using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade
{
    [XmlRoot(ElementName = "LogEntry", Namespace = XmlNamespaces.Default)]
    public class LogEntry
    {
        [XmlElement(ElementName="id", Order=0)]
        public Identity Id { get; set; }

        [XmlElement(ElementName = "utcTs", Order = 1)]
        public DateTime UtcTs { get; set; }

        [XmlElement(ElementName = "type", Order = 2)]
        public LogEntryType Type { get; set; }

        [XmlElement(ElementName="message", Order=3)]
        public string Message { get; set; }
    }
}
