using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Repository
{
    public interface ISubscriptionRepository
    {
        Subscription Create(Subscription instance);
        Subscription Update(Subscription instance);
        Subscription Get(Guid id);
        void Delete(Guid id);
        bool ExistsById(Guid id);
        bool IsQueryValid(string filter);
        IEnumerable<Subscription> ForTopic(Guid topicId);
        IEnumerable<Subscription> ForGroup(Guid groupId);
        IEnumerable<Subscription> Find(string query, int? skip, int? limit);
    }
}
