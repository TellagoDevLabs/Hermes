using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Service
{
    public interface ISubscriptionService
    {
        Subscription Create(Subscription subscription);
        Subscription Get(Identity id);
        Subscription Update(Subscription subscription);
        void Delete(Identity id);
        bool ExistsById(Identity id);
        IEnumerable<Subscription> Find(string query, int? skip = null, int? limit = null);
        IEnumerable<Subscription> GetByTopic(Identity topicId);
        IEnumerable<Subscription> GetByGroup(Identity groupId);
        IEnumerable<Subscription> GetByTopicAndTopicsGroups(Identity topicId);
    }
}