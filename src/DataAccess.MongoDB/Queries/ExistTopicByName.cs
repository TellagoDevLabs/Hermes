using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class ExistTopicByName : MongoDbRepository, IExistsTopicByName
    {
        public ExistTopicByName(string connectionString) 
            : base(connectionString)
        {}

        public bool Execute(string topicName, Identity? excludeId = null)
        {
            var query = QueryDuplicatedName(topicName, excludeId);
            return DB.GetCollection(MongoDbConstants.Collections.Topics).Exists(query);
        }

        public IMongoQuery QueryDuplicatedName(string topicName, Identity? excludeId = null)
        {
            return excludeId.HasValue ?
                Query.And(Query.EQ("Name", BsonString.Create(topicName)), Query.NE("_id", excludeId.Value.ToBson())) :
                Query.EQ("Name", BsonString.Create(topicName));
        }
    }
}