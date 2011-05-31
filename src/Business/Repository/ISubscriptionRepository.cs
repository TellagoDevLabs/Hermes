using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface ISubscriptionRepository : ISubscriptionQueries
    {
        Subscription Create(Subscription instance);
        Subscription Update(Subscription instance);
        Subscription Get(Identity id);
        void Delete(Identity id);
        bool ExistsById(Identity id);
        bool IsQueryValid(string filter);
        IEnumerable<Subscription> ForTopic(Identity topicId);
        IEnumerable<Subscription> ForGroup(Identity groupId);
        IEnumerable<Subscription> Find(string query, int? skip, int? limit);
    }
}
