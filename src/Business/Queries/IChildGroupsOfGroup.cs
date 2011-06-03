using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Queries
{
    public interface IChildGroupsOfGroup
    {
        bool HasChilds(Group group);
        IEnumerable<Group> GetChilds(Group @group);
    }
}