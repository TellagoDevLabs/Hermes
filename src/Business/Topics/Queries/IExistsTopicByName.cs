using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Topics.Queries
{
    public interface IExistsTopicByName
    {
        bool Execute(string topicName, Identity? excludeId = null);
    }
}