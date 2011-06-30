using System.Collections.Generic;
using System.IO;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client.Util
{
    public class Message
    {
        public Message()
        {
            Headers = new Header[0];
        }

        public Identity TopicId {get;set;}
        public Stream Payload {get; set;}
        public IEnumerable<Header> Headers { get; set; }
    }
}
