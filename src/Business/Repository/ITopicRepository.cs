using System;
using TellagoStudios.Hermes.Business.Model;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface ITopicRepository : ITopicQueries
    {
        Topic Create(Topic topic);
        Topic Get(Identity id);
        Topic Update(Topic topic);
        void Delete(Identity id);
        IEnumerable<Topic> Find(string query, int? skip, int? limit);
        IEnumerable<Identity> GetTopicIdsInGroup(Identity groupId);
        bool ExistsById(Identity id);
        bool ExistsByQuery(string query);
    }
}