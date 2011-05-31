using System;
using TellagoStudios.Hermes.Business.Model;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface IGroupRepository : IGroupQueries
    {
        Group Create(Group topic);
        Group Get(Identity id);
        Group Update(Group topic);
        void Delete(Identity id);
        IEnumerable<Group> Find(string query, int? skip, int? limit);
        bool ExistsById(Identity id);
        bool ExistsByQuery(string query);
    }
}