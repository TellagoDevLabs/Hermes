using System;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface IGroupQueries
    {
        string QueryDuplicatedName(Group group);
        string QueryGetChildGroups(Identity id);
    }
}
