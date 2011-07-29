using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IExistsTopicByName
    {
        bool Execute(Identity? groupId, string topicName, Identity? excludeId = null);
    }
}