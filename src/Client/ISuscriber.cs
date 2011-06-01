using System.Net;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public interface ISuscriber
    {        
        void CreateSubscription(SubscriptionPost post);
        void UpdateSubscription(SubscriptionPut put);
        void DeleteSubscription(Identity subscriptionId);

        Subscription[] GetSubscriptions();
        Subscription[] GetSubscriptionsByTopic(Identity topicId);
        Subscription[] GetSubscriptionsByGroup(Identity groupId);

        Link[] GetMessagesLink(Identity subscriptionId);        
        HttpWebResponse GetMessage(string href);
        HttpWebResponse GetMessage(Identity topicId, Identity messageId);
    }
}