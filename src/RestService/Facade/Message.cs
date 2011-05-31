using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Serialization;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Facade
{
    [XmlRoot(ElementName = "Message", Namespace = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade")]
    public class Message
    {
        public Message()
        {
            Headers = new List<Header>();
            PromotedProperties = new List<Header> ();
        }

        public Identity Id { get; set; }

        // Post content
        public HttpContent Payload { get; set; }       

        // All headers that were received with the POST
        public List<Header> Headers { get; set; }

        // Promoted properties received with the POST
        public List<Header> PromotedProperties { get; set; }

        // The topic that the message belongs to
        public Identity TopicId { get; set; }

        //public string ContentType { get; set; }
    }
}