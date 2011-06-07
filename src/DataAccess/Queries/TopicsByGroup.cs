using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
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

        public IEnumerable<Topic> GetTopics(Identity id, int? skip = null, int? limit = null) 
        {
            var cursor =  topicsCollection.Find(QueryGetByGroup(id));
            if (skip.HasValue) cursor.SetSkip(skip.Value);
            if (limit.HasValue) cursor.SetSkip(limit.Value);
            return cursor;
        }

        private static QueryDocument QueryGetByGroup(Identity groupId)
        {
            var query = "{\"GroupId\" : " + groupId.ToBsonString() + "}";
            return query.ToQueryDocument();

        }
    }
}