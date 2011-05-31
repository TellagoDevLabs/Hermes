using System;
using TellagoStudios.Hermes.Common.Model;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Common.Repository
{
    public interface ITopicRepository
    {
        Topic Create(Topic topic);
        Topic Get(Guid id);
        Topic Update(Topic topic);
        void Delete(Guid id);
        IEnumerable<Topic> Find(string query, int? skip, int? limit);
        IEnumerable<Guid> GetTopicIdsInGroup(Guid groupId);
        bool ExistsById(Guid id);
        bool ExistsByQuery(string query);
    }
}