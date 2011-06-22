
using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class ExistGroupByGroupName : MongoDbRepository, IExistsGroupByGroupName
    {
        public ExistGroupByGroupName(string connectionString) 
            : base(connectionString)
        {}

        public bool Execute(string groupName, Identity? excludeId = null)
        {
            var query = QueryDuplicatedName(groupName, excludeId);
            return DB.GetCollection(MongoDbConstants.Collections.Groups).Exists(query);
        }

        public IMongoQuery QueryDuplicatedName(string groupName, Identity? excludeId = null)
        {
            return excludeId.HasValue ?
                Query.And(Query.EQ("Name", BsonString.Create(groupName)), Query.NE("_id", excludeId.Value.ToBson())) :
                Query.EQ("Name", BsonString.Create(groupName));
        }
    }
}