using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IGetWorkingFeedForTopic
    {
        Feed Execute(Identity topicId);
    }
}