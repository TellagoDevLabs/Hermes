using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IExistGroupByGroupName
    {
        bool Execute(string groupName, Identity? excludeId = null);
    }
}