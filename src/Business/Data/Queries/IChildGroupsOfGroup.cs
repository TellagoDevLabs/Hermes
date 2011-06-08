using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IChildGroupsOfGroup
    {
        bool HasChilds(Identity id);
        IEnumerable<Group> GetChildren(Identity id);
        IEnumerable<Identity> GetChildrenIds(Identity id);
    }
}