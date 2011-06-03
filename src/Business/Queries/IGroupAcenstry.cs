using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Queries
{
    public interface IGroupAcenstry
    {
        IEnumerable<Group> Execute(Group group);
    }
}