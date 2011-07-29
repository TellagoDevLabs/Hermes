using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class GroupByName : MongoDbRepository, IGroupByName
    {
        public GroupByName(string connectionString) 
            : base(connectionString)
        {}

        public bool Exists(string name)
        {
            var query = GetQuery(name);
            return DB.GetCollectionByType<Group>().Exists(query);
        }

        public Group Get(string name)
        {
            var query = GetQuery(name);
            return DB.GetCollectionByType<Group>().FindOne(query);
        }

        private static IMongoQuery GetQuery(string name)
        {
            return Query.EQ("Name", BsonValue.Create(name));
        }
    }
}