using System;
using TellagoStudios.Hermes.Common.Model;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Common.Repository
{
    public interface IGroupRepository
    {
        Group Create(Group topic);
        Group Get(Guid id);
        Group Update(Group topic);
        void Delete(Guid id);
        IEnumerable<Group> Find(string query, int? skip, int? limit);
        bool ExistsById(Guid id);
        bool ExistsByQuery(string query);
    }
}