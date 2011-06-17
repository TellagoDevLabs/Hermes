using System;
using System.Linq;
using System.Threading.Tasks;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Events;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Pushing
{
    public class NewMessagePusher : IEventHandler<NewMessageEvent>
    {
        public ISubscriptionsByTopicAndTopicGroup SubscriptionsByTopicAndGroup { get; set; }
        public IMessageByMessageKey Repository { get; set; }

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
            var subscriptions = SubscriptionsByTopicAndGroup.Execute(message.TopicId);

            var filteredSubscriptions = subscriptions
                .Where(s => string.IsNullOrWhiteSpace(s.Filter) ||
                            Repository.Exist(message.ToMessageKey(), s.Filter));

            foreach (var subscription in filteredSubscriptions)
            {
                try
                {
                    message.PushToSubscription(subscription);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(
                        string.Format(Business.Texts.ErrorPushingCallback, message.Id, subscription.Id) +
                        "\r\n" + ex);
                    RetryService.Add(new Retry(message, subscription));
                }
            }
        }
    }
}
