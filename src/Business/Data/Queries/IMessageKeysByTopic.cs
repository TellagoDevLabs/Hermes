using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IMessageKeysByTopic
    {
        IEnumerable<MessageKey> Get(Identity topicId, Identity? last = null, int? skip = null, int? limit = null) ;
    }
}