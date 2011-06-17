using System;

namespace TellagoStudios.Hermes.Business.Model
{
    public class Retry : EntityBase
    {

        public Retry()
        {
        }

        public Retry(Message message, Subscription subscription)
        {
            Message = message;
            Subscription = subscription;
        }

        public int Count { get; set; }        
        // UTC date time when the message was tried to send last time
        public DateTime UtcLastTry { get; set; }

        public Message Message { get; set; }
        public Subscription Subscription { get; set; }               
    }
}
