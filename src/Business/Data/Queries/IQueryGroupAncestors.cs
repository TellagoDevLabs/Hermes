using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IQueryGroupAncestors
    {
        IEnumerable<Group> Execute(Identity groupId);
    }
}