using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class SubscriptionsByTopic : MongoDbRepository, ISubscriptionsByTopic
    {
        private readonly MongoCollection<Subscription> subscriptionsCollection;

        public SubscriptionsByTopic(string connectionString)
            : base(connectionString)
        {
            subscriptionsCollection = DB.GetCollectionByType<Subscription>();
        }

        #region ISubscriptionsByGroup Members

        public IEnumerable<Subscription> Execute(Identity topicId)
        {
            return subscriptionsCollection.Find(CreateQueryDocument(topicId));
        }

        #endregion
        
        public IMongoQuery CreateQueryDocument(Identity topicId)
        {
            return Query.And(
                        Query.NE("Callback", BsonNull.Value),
                        Query.EQ("TargetKind", (int)TargetKind.Topic),
                        Query.EQ("TargetId", topicId.ToBson()));
        }
    }
}