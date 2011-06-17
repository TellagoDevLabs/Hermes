using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class SubscriptionsByTopicAndTopicGroup : MongoDbRepository, ISubscriptionsByTopicAndTopicGroup
    {
        private readonly IQueryGroupAncestors queryGroupAncestors;
        private readonly MongoCollection<Topic> topicCollection;
        private readonly MongoCollection<Subscription> subscriptionCollection;

        public SubscriptionsByTopicAndTopicGroup(string connectionString, IQueryGroupAncestors queryGroupAncestors) : base(connectionString)
        {
            this.queryGroupAncestors = queryGroupAncestors;
            var topicCollectionName = MongoDbConstants.GetCollectionNameForType<Topic>();
            topicCollection = DB.GetCollection<Topic>(topicCollectionName);
            var subscriptionCollectionName = MongoDbConstants.GetCollectionNameForType<Subscription>();
            subscriptionCollection = DB.GetCollection<Subscription>(subscriptionCollectionName);
        }

        private static QueryDocument QueryGetByTopic(Identity topicId)
        {
            string queryGetByTopic = string.Format("{{\"Callback\":{{$ne:null}}, \"TargetKind\":{0}, \"TargetId\" : {1}}}", 
                                                    (int)TargetKind.Topic, topicId.ToBsonString());
            return queryGetByTopic.ToQueryDocument();
        }

        private static QueryDocument QueryGetByGroup(Identity groupId)
        {
            string queryGetByGroup = string.Format("{{\"Callback\":{{$ne:null}}, \"TargetKind\":{0}, \"TargetId\" : {1}}}", 
                                                    (int)TargetKind.Group, groupId.ToBsonString());
            return queryGetByGroup.ToQueryDocument();
        }

        public IEnumerable<Subscription> Execute(Identity topicId)
        {
            var topic = topicCollection.FindById(topicId);
            var subscriptions = subscriptionCollection.Find(QueryGetByTopic(topicId)).ToList();
            var groups = queryGroupAncestors.Execute(topic.GroupId);
            subscriptions.AddRange(groups.SelectMany(g => subscriptionCollection.Find(QueryGetByGroup(g.Id.Value))));
            return subscriptions.Distinct();
            
        }
    }
}