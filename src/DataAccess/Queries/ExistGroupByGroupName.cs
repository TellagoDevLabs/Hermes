using System;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class ExistGroupByGroupName : MongoDbRepository, IExistGroupByGroupName
    {
        public ExistGroupByGroupName(string connectionString) 
            : base(connectionString)
        {}

        public bool Execute(string groupName, Identity? excludeId = null)
        {
            var query = QueryDuplicatedName(groupName, excludeId);
            return DB.GetCollection(MongoDbConstants.Collections.Groups).Exists(query.ToQueryDocument());
        }

        public string QueryDuplicatedName(string groupName, Identity? excludeId = null)
        {
            return excludeId.HasValue ?
                "{ \"Name\":\"" + groupName + "\", \"_id\" : { $ne : " + excludeId.Value.ToBsonString() + "} }" :
                "{ \"Name\":\"" + groupName + "\"}";
        }
    }
}