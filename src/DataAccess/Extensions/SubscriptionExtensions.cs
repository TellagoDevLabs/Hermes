using System;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Common;
using TellagoStudios.Hermes.DataAccess;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.Common.Model
{
    public static class SubscriptionExtensions
    {
        public static BsonDocument ToMongoDocument(this Subscription subscription)
        {
            if (subscription == null) return null;

            var doc = new BsonDocument();

            if (!String.IsNullOrEmpty(subscription.Id))
            {
                doc[MongoDbSubscriptionRepository.FieldNames.Id] = new BsonObjectId(subscription.Id);
            }

            switch(subscription.TargetKind)
            {
                case TargetKind.None:
                    break;
                case TargetKind.Topic:
                    doc[MongoDbSubscriptionRepository.FieldNames.TargetRef] = subscription.TargetId.ToDBRef(Constants.Relationships.Topic).ToBsonDocument();
                    break;
                case TargetKind.Group:
                    doc[MongoDbSubscriptionRepository.FieldNames.TargetRef] = subscription.TargetId.ToDBRef(Constants.Relationships.Group).ToBsonDocument();
                    break;
                default:
                    throw new InvalidCastException(string.Format(Messages.InvalidTargetKind, subscription.TargetKind));
            }

            if (subscription.Callback!=null)
            {
                var callback = new BsonDocument();
                callback[MongoDbSubscriptionRepository.FieldNames.CallbackUrl] = subscription.Callback.Url.ToString();
                callback[MongoDbSubscriptionRepository.FieldNames.CallbackKind] = subscription.Callback.Kind.ToString();
                doc[MongoDbSubscriptionRepository.FieldNames.Callback] = callback;
            }

            if (!string.IsNullOrWhiteSpace(subscription.Filter))
            {
                doc[MongoDbSubscriptionRepository.FieldNames.Filter] =
                    subscription.Filter.ToQueryDocument().AsBsonDocument;
            }

            return doc;
        }

        public static Subscription ToSubscription(this BsonDocument doc)
        {
            if (doc == null) return null;

            var subscription = new Subscription { Id = doc[MongoDbSubscriptionRepository.FieldNames.Id].ToString() };
                
            if (doc.Contains(MongoDbSubscriptionRepository.FieldNames.Callback))
            {
                var callback = doc[MongoDbSubscriptionRepository.FieldNames.Callback].AsBsonDocument;
                subscription.Callback = new Callback
                                            {
                                                Kind = (CallbackKind)Enum.Parse(typeof (CallbackKind), callback[MongoDbSubscriptionRepository.FieldNames.CallbackKind].AsString),
                                                Url = new Uri(callback[MongoDbSubscriptionRepository.FieldNames.CallbackUrl].AsString)
                                            };
            }

            var targetRef = doc[MongoDbSubscriptionRepository.FieldNames.TargetRef].AsBsonDocument;
            subscription.TargetId = targetRef["$id"].ToString();
            switch(targetRef["$ref"].AsString)
            {
                case Constants.Relationships.Topic:
                    subscription.TargetKind = TargetKind.Topic;
                    break;
                case Constants.Relationships.Group:
                    subscription.TargetKind = TargetKind.Group;
                    break;
                default:
                    throw new InvalidCastException(string.Format(Messages.InvalidTargetKind, targetRef["$ref"]));
            }

            if (doc.Contains(MongoDbSubscriptionRepository.FieldNames.Filter))
            {
                subscription.Filter = doc[MongoDbSubscriptionRepository.FieldNames.Filter].AsBsonDocument.ToJson();
            }
            return subscription;
        }
    }
}
