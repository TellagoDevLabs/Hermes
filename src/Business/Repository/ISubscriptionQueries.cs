using System;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface ISubscriptionQueries
    {
        string QueryGetByGroup(Identity groupId);
        string QueryGetByTopic(Identity topicId);
    }
}
