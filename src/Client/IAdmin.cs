using System;
using System.Net;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public interface IAdmin
    {
        void CreateGroup(GroupPost groupPost);
        void UpdateGroup(GroupPut groupPut);
        void DeleteGroup(Identity groupId);
        Group[] GetGroups();

        void CreateTopic(TopicPost topicPost);        
        void UpdateTopic(TopicPut topicPut);
        void DeleteTopic(Identity topicId);
        Topic[] GetTopicsByGroup(Identity groupId);

        void CreateSubscription(SubscriptionPost post);
        void UpdateSubscription(SubscriptionPut put);
        void DeleteSubscription(Identity subscriptionId);
        Subscription[] GetSubscriptions();
        Subscription[] GetSubscriptionsByTopic(Identity topicId);
        Subscription[] GetSubscriptionsByGroup(Identity groupId);

        Uri PostMessage(Message message);
        Link[] GetMessagesLink(Identity subscriptionId);
        HttpWebResponse GetMessage(string href);
        HttpWebResponse GetMessage(Identity topicId, Identity messageId);
    }
}