using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Service
{
    public interface ITopicService
    {
        Topic Create(Topic topic);
        Topic Get(Guid id);
        Topic Update(Topic topic);
        void Delete(Guid id);
        IEnumerable<Topic> Find(string query, int? skip = null, int? limit = null);
        IEnumerable<Guid> GetTopicIdsInGroup(Guid groupId);
        bool Exists(Guid id);
        bool ExistsByGroup(Guid groupId);
    }
}