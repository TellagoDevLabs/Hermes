using System;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class ExistTopicByName : MongoDbRepository, IExistsTopicByName
    {
        public ExistTopicByName(string connectionString) 
            : base(connectionString)
        {}

        public bool Execute(string topicName, Identity? excludeId = null)
        {
            var query = QueryDuplicatedName(topicName, excludeId);
            return DB.GetCollection(MongoDbConstants.Collections.Topics).Exists(query.ToQueryDocument());
        }

        public string QueryDuplicatedName(string topicName, Identity? excludeId = null)
        {
            return excludeId.HasValue ?
                "{ \"Name\":\"" + topicName + "\", \"_id\" : { $ne : " + excludeId.Value.ToBsonString() + "} }" :
                "{ \"Name\":\"" + topicName + "\"}";
        }
    }
}