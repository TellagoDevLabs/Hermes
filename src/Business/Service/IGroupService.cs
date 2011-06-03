using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Service
{
    public interface IGroupService
    {
        Group Get(Identity id);
        IEnumerable<Group> Find(string query, int? skip, int? limit);
        bool Exists(Identity id);
        IEnumerable<Group> GetAncestry(Identity groupId);
    }
}