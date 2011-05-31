using System;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface ITopicQueries
    {
        string QueryGetByGroup(Identity groupId);
        string QueryDuplicatedName(Topic topic);
    }
}
