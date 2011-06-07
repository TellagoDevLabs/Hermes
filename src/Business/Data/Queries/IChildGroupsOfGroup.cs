using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IChildGroupsOfGroup
    {
        bool HasChilds(Identity id);
        IEnumerable<Group> GetChilds(Identity id);
    }
}