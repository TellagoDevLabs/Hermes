using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class TopicByName : MongoDbRepository, ITopicByName
    {
        public TopicByName(string connectionString)
            : base(connectionString)
        { }

        public bool Exists(string name, Identity? groupId)
        {
            var query = GetQuery(name, groupId);
            return DB.GetCollectionByType<Topic>().Exists(query);
        }

        public Topic Get(string name, Identity? groupId)
        {
            var query = GetQuery(name, groupId);
            return DB.GetCollectionByType<Topic>().FindOne(query);
        }

        private static IMongoQuery GetQuery(string name, Identity? groupId)
        {
            return Query.And(
                Query.EQ("Name", BsonValue.Create(name)),
                Query.EQ("GroupId", groupId.ToBson()));
        }
    }

}