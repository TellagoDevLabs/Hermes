using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Extensions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Validator;
using System.Threading.Tasks;

namespace TellagoStudios.Hermes.Business.Service
{
    public class MessageService : IMessageService
    {
        public IMessageRepository Repository { get; set; }
        public MessageValidator Validator { get; set; }
        public IGroupService GroupService { get; set; }
        public ITopicService TopicService { get; set; }
        public ISubscriptionService SubscriptionService { get; set; }
        public IRetryService RetryService { get; set; }
        public ILogService LogService  { get; set; }

        public Message Create(Message message)
        {
            Guard.Instance.ArgumentNotNull(()=>message, message);

            Validator.ValidateBeforeCreate(message);
            var result = Repository.Create(message);

            // Push message to subscribers (callback)
            Task.Factory.StartNew(() => PushToSubscribers(result), TaskCreationOptions.None);

            return result;
        }

        public IEnumerable<Message> CreateByGroup(Identity groupId, Message message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MessageKey> GetMessageKeysBySubscription(Identity subscriptionId)
        {
            Guard.Instance.ArgumentNotNull(()=>subscriptionId, subscriptionId);

            Validator.ValidateSubsriptionExists(subscriptionId);

            var subscription = SubscriptionService.Get(subscriptionId);
            var topicIds = new List<Identity>();

            switch (subscription.TargetKind)
            {
                case TargetKind.Topic:
                    topicIds.Add(subscription.TargetId.Value);
                    break;
                case TargetKind.Group:
                    AddTopicIdsFromGroup(topicIds, subscription.TargetId.Value);
                    break;
                default:
                    throw new InvalidOperationException(string.Format(Messages.TargetKindUnknown, subscription.TargetKind));
            }

            // For each topic in collection, gets each message that matchs the filter and returns message's key
            return topicIds.SelectMany(topicId => Repository.GetMessageKeys(topicId, subscription.Filter));
        }

        public Message Get(MessageKey key)
        {
            Guard.Instance.ArgumentNotNull(()=>key, key);

            Validator.ValidateBeforeGet(key);

            return Repository.Get(key);
        }

        public IEnumerable<MessageKey> GetForTopic(Identity topicId)
        {
            Guard.Instance.ArgumentNotNull(()=>topicId, topicId);

            return Repository.GetMessageKeys(topicId);
        }

        public IEnumerable<MessageKey> GetForGroup(Identity groupId)
        {
            Guard.Instance.ArgumentNotNull(()=>groupId, groupId);

            var topicIds = TopicService.GetTopicIdsInGroup(groupId);
            var forGroup = topicIds.SelectMany(id => Repository.GetMessageKeys(id)).ToList();

            return forGroup;
        }

        public bool Exists(MessageKey key, string filter)
        {
            Guard.Instance.ArgumentNotNull(()=>key, key);

            return Repository.Exists(key, filter);
        }

        #region Private methods

        private void AddTopicIdsFromGroup(List<Identity> topicIds, Identity groupId)
        {
            var topics = TopicService.GetTopicIdsInGroup(groupId);
            topicIds.AddRange(topics);
            
            var group = GroupService.Get(groupId);
            if (group != null && group.ParentId.HasValue)
            {
                AddTopicIdsFromGroup(topicIds, group.ParentId.Value);
            }
        }

        private void PushToSubscribers(Message message)
        {
            var subscriptions = SubscriptionService.GetByTopicAndTopicsGroups(message.TopicId);
            
            var filteredSubscriptions = subscriptions
                .Where(s => string.IsNullOrWhiteSpace(s.Filter) || 
                            Repository.Exists(message.ToMessageKey(), s.Filter));

            foreach (var subscription in filteredSubscriptions)
            {
                try
                {
                    message.PushToSubscription(subscription);
                }
                catch (Exception ex)
                {
                    LogService.LogError(string.Format(Messages.ErrorPushingCallback, message.Id, subscription.Id), ex);
                    RetryService.Add(new Retry(message, subscription));
                }
            }
        }

        #endregion
    }
}