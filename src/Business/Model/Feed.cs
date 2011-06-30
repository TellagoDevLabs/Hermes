using System;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Business.Model
{
    public class Feed : EntityBase
    {
        public Feed()
        {
            Entries = new List<FeedEntry>();
        }

        public Identity TopicId { get; set; }
        public DateTime Updated { get; set; }
        public List<FeedEntry> Entries { get; set; }
    }

    public class FeedEntry
    {
        public Identity MessageId { get; set; }
        public DateTime TimeStamp { get; set; } 
    }
}