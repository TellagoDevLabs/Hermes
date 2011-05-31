using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TellagoStudios.Hermes.DataAccess.MongoDB
{
    public class MongoDbSubscriptionRepository : MongoDbRepository, ISubscriptionRepository
    {
        private readonly MongoCollection<Subscription> _subscriptionsCollection;

        public MongoDbSubscriptionRepository(string connectionString)
            :base(connectionString)
        {
            _subscriptionsCollection = DB.GetCollection<Subscription>(MongoDbConstants.Collections.Subscriptions);
        }

        public Subscription Create(Subscription subscription)
        {
            Guard.Instance.ArgumentNotNull(()=>subscription, subscription);            

            _subscriptionsCollection.Save(subscription);
            return subscription;
        }

        public Subscription Update(Subscription subscription)
        {
            Guard.Instance.ArgumentNotNull(()=>subscription, subscription);            

            _subscriptionsCollection.Save(subscription);
            return subscription;
        }

        public Subscription Get(Identity id)
        {
            var result = _subscriptionsCollection.FindById(id);

            return result;
        }

        public void Delete(Identity id)
        {
            _subscriptionsCollection.Remove(id);
        }

        public bool ExistsById(Identity id)
        {
            return _subscriptionsCollection.Exists(id);
        }

        public bool IsQueryValid(string filter)
        {
            try
            {
                filter.ToQueryDocument();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Subscription> ForTopic(Identity topicId)
        {
            var query = QueryGetByTopic(topicId);
            return Find(query, null, null);
        }

        public IEnumerable<Subscription> ForGroup(Identity groupId)
        {
            var query = QueryGetByGroup(groupId);
            return Find(query, null, null);
        }

        public IEnumerable<Subscription> Find(string query, int? skip, int? limit)
        {
            if (skip.HasValue && skip.Value < 0) throw new ArgumentOutOfRangeException("skip");
            if (limit.HasValue && limit.Value <= 0) throw new ArgumentOutOfRangeException("limit");

            var queryDoc = query.ToQueryDocument();

            var cursor = _subscriptionsCollection.Find(queryDoc);

            if (skip.HasValue && skip.Value > 0) cursor = cursor.SetSkip(skip.Value);
            if (limit.HasValue && limit.Value >= 0) cursor = cursor.SetLimit(limit.Value);

            return cursor;
        }


        public string QueryGetByGroup(Identity groupId)
        {
            return "{\"Callback\":{$ne:null}, \"TargetKind\":" + (int)TargetKind.Group + ", \"TargetId\" : " + groupId.ToBsonString() + "}";
        }

        public string QueryGetByTopic(Identity topicId)
        {
            return "{\"Callback\":{$ne:null}, \"TargetKind\":" + (int)TargetKind.Topic + ", \"TargetId\" : " + topicId.ToBsonString() + "}";
        }
    }
}
