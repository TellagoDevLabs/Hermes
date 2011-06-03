using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class TopicsByGroup : MongoDbRepository, ITopicsByGroup
    {
        private readonly MongoCollection<Topic> topicsCollection;

        public TopicsByGroup(string connectionString) : base(connectionString)
        {
            topicsCollection = DB.GetCollection<Topic>(MongoDbConstants.Collections.Topics);
        }

        public bool HasTopics(Group group)
        {
            if(!group.Id.HasValue ) throw new InvalidOperationException("Group without id.");
            return topicsCollection.Exists(QueryGetByGroup(group.Id.Value));
        }

        public IEnumerable<Topic> GetTopics(Group group)
        {
            if (!group.Id.HasValue) throw new InvalidOperationException("Group without id.");
            return topicsCollection.Find(QueryGetByGroup(group.Id.Value));
        }

        private static QueryDocument QueryGetByGroup(Identity groupId)
        {
            var query = "{\"GroupId\" : " + groupId.ToBsonString() + "}";
            return query.ToQueryDocument();

        }
    }
}