using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Queries
{
    public interface ITopicsByGroup
    {
        bool HasTopics(Group @group);
        IEnumerable<Topic> GetTopics(Group @group);
    }
}