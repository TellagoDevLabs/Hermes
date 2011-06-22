using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class SubscriptionsByGroup : MongoDbRepository, ISubscriptionsByGroup
    {
        private readonly MongoCollection<Subscription> subscriptionsCollection;

        public SubscriptionsByGroup(string connectionString) : base(connectionString)
        {
            subscriptionsCollection = DB.GetCollectionByType<Subscription>();
        }

        #region ISubscriptionsByGroup Members

        public IEnumerable<Subscription> Execute(Identity groupId)
        {
            return subscriptionsCollection.Find(CreateQueryDocument(groupId));
        }

        #endregion

        public IMongoQuery CreateQueryDocument(Identity groupId)
        {
            return Query.And(
                        Query.NE("Callback", BsonNull.Value), 
                        Query.EQ("TargetKind", (int)TargetKind.Group),
                        Query.EQ("TargetId", groupId.ToBson()));
        }
    }
}