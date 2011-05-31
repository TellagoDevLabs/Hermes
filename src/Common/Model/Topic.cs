
using System;

namespace TellagoStudios.Hermes.Common.Model
{
    public class Topic 
    {
        public Guid? Id { get; set; }
        public Guid GroupId { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string MessagesCollectionName 
        {
            get { return Id==null ? null : "msg_" + Id;  }
        }
    }
}