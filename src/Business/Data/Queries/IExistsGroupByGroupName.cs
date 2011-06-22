using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IExistsGroupByGroupName
    {
        bool Execute(string groupName, Identity? excludeId = null);
    }
}