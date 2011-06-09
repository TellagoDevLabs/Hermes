using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface ISubscriptionsByTopic
    {
        IEnumerable<Subscription> Execute(Identity topicId);
    }
}