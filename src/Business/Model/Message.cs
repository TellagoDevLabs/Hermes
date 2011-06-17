using System;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Business.Model
{
    public class Message
    {
        public Message()
        {
            Headers = new Dictionary<string, string[]>();
            PromotedProperties = new Dictionary<string, string>();
        }

        public Identity? Id { get; set; }

        // Post content
        public byte[] Payload { get; set; }

        // All headers that were received with the POST
        public Dictionary<string, string[]> Headers { get; set; }

        // Promoted properties received with the POST
        public Dictionary<string, string> PromotedProperties { get; set; }

        // UTC date time when the message was posted
        public DateTime UtcReceivedOn { get; set; }

        // The topic that the message belongs to
        public Identity TopicId { get; set; }
    }
}
