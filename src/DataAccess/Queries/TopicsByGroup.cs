using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
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

        public bool HasTopics(Identity id)
        {
            return topicsCollection.Exists(QueryGetByGroup(id));
        }

        public IEnumerable<Topic> GetTopics(Identity groupId, int? skip = null, int? limit = null)
        {
            var cursor = topicsCollection.Find(QueryGetByGroup(groupId));
            if (skip.HasValue) cursor.SetSkip(skip.Value);
            if (limit.HasValue) cursor.SetSkip(limit.Value);
            return cursor;
        }

        public IEnumerable<Identity> GetTopicIds(Identity groupId, int? skip = null, int? limit = null)
        {
            var cursor = topicsCollection.Find(QueryGetByGroup(groupId));
            cursor.SetFields("_id");

            if (skip.HasValue) cursor.SetSkip(skip.Value);
            if (limit.HasValue) cursor.SetSkip(limit.Value);

            return cursor.Select(doc => doc.Id.Value);
        }

        private static IMongoQuery QueryGetByGroup(Identity groupId)
        {
            return Query.EQ("GroupId", groupId.ToBson());
        }
    }
}