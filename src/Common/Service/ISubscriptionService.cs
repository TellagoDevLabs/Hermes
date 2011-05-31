using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Service
{
    public interface ISubscriptionService
    {
        Subscription Create(Subscription subscription);
        Subscription Get(Guid id);
        Subscription Update(Subscription subscription);
        void Delete(Guid id);
        bool ExistsById(Guid id);
        IEnumerable<Subscription> Find(string query, int? skip = null, int? limit = null);
        IEnumerable<Subscription> GetSubscriptionsForTopic(Guid topicId);
    }
}