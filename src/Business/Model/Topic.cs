
using System;

namespace TellagoStudios.Hermes.Business.Model
{
    public class Topic 
    {
        public Identity? Id { get; set; }
        public Identity GroupId { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string MessagesCollectionName 
        {
            get { return Id==null ? null : "msg_" + Id;  }
        }
    }
}