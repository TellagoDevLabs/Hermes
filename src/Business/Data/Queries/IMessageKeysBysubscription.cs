using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IMessageKeysBySubscription
    {
        IEnumerable<MessageKey> Get(Identity subscriptionId, int? skip = null, int? limit = null) ;
    }
}