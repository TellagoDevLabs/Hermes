using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Service
{
    public interface IGroupService
    {
        Group Create(Group group);
        Group Get(Guid id);
        Group Update(Group group);
        void Delete(Guid id);
        IEnumerable<Group> Find(string query, int? skip, int? limit);
        bool Exists(Guid id);
        IEnumerable<Group> GetAncestry(Guid groupId);
    }
}