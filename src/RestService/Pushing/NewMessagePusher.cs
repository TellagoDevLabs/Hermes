using System;
using System.Linq;
using System.Threading.Tasks;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Events;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Service;

namespace TellagoStudios.Hermes.RestService.Pushing
{
    public class NewMessagePusher : IEventHandler<NewMessageEvent>
    {
        public ISubscriptionService SubscriptionService { get; set; }
        public IMessageRepository Repository { get; set; }
        public ILogService LogService { get; set; }
        public IRetryService RetryService { get; set; }

        public void Handle(NewMessageEvent @event)
        {
            Task.Factory.StartNew(() => Push(@event.Message), TaskCreationOptions.None);
        }

        public Type Type
        {
            get { return typeof(NewMessageEvent); }
        }

        public void Handle(object @event)
        {
            Handle((NewMessageEvent)@event);
        }

        private void Push(Message message)
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
                    LogService.LogError(
                        string.Format(Business.Messages.ErrorPushingCallback, message.Id, subscription.Id), ex);
                    RetryService.Add(new Retry(message, subscription));
                }
            }
        }
    }
}
