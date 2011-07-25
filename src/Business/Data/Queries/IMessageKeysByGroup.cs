using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IMessageKeysByGroup
    {
        IEnumerable<MessageKey> Get(Identity groupId, Identity? last = null, int? skip = null, int? limit = null) ;
    }
}