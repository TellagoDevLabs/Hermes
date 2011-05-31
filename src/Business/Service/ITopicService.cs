using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Service
{
    public interface ITopicService
    {
        Topic Create(Topic topic);
        Topic Get(Identity id);
        Topic Update(Topic topic);
        void Delete(Identity id);
        IEnumerable<Topic> Find(string query, int? skip = null, int? limit = null);
        IEnumerable<Topic> GetByGroup(Identity groupId, int? skip = null, int? limit = null);
        IEnumerable<Identity> GetTopicIdsInGroup(Identity groupId);
        bool Exists(Identity id);
        bool ExistsByGroup(Identity groupId);
    }
}