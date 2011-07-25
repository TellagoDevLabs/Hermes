using System;
using System.Collections.Generic;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class MessageKeysBySubscription  : MongoDbRepository, IMessageKeysBySubscription
    {
        private IMessageKeysByTopic messageByTopic;
        private IMessageKeysByGroup messageByGroup;
        private IEntityById entityById;

        public MessageKeysBySubscription(string connectionString, IEntityById entityById, IMessageKeysByTopic messageByTopic, IMessageKeysByGroup messageByGroup)
            : base(connectionString)
        {
            this.entityById = entityById;
            this.messageByTopic = messageByTopic;
            this.messageByGroup = messageByGroup;
        }

        public IEnumerable<MessageKey> Get(Identity subscriptionId, Identity? last = null, int? skip = null, int? limit = null)
        {
            var subscription = entityById.Get<Subscription>(subscriptionId);

            if (subscription != null)
            {
                switch (subscription.TargetKind)
                {
                    case TargetKind.Topic:
                        return messageByTopic.Get(subscription.TargetId.Value, last, skip, limit);
                    case TargetKind.Group:
                        return messageByGroup.Get(subscription.TargetId.Value, last, skip, limit);
                    default:
                        throw new InvalidOperationException(string.Format(Texts.TargetKindUnknown,
                                                                          subscription.TargetKind));
                }
            }

            return new MessageKey[0];
        }
    }
}