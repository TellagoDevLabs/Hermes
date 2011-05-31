using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Service
{
    public interface IGroupService
    {
        Group Create(Group group);
        Group Get(Identity id);
        Group Update(Group group);
        void Delete(Identity id);
        IEnumerable<Group> Find(string query, int? skip, int? limit);
        bool Exists(Identity id);
        IEnumerable<Group> GetAncestry(Identity groupId);
    }
}