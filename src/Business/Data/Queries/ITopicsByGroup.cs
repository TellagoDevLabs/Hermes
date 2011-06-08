using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface ITopicsByGroup
    {
        bool HasTopics(Identity id);
        IEnumerable<Topic> GetTopics(Identity id, int? skip = null, int? limit = null) ;
        IEnumerable<Identity> GetTopicIds(Identity groupId, int? skip = null, int? limit = null);
    }
}