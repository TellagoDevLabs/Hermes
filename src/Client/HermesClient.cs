using System;
using System.Net;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public class HermesClient
    {
        private readonly RestClient restClient;

        #region Constructors

        public HermesClient(string hermesAddress)
        {
            Guard.Instance.ArgumentNotNullOrEmpty(() => hermesAddress, hermesAddress);

            var address = new Uri(hermesAddress);
            restClient = new RestClient(address);
        }

        public HermesClient(Uri hermesAddress)
        {
            Guard.Instance.ArgumentNotNull(() => hermesAddress, hermesAddress);
            restClient = new RestClient(hermesAddress);
        }

        public void CreateTopic(TopicPost topicPost)
        {
            Guard.Instance.ArgumentNotNull(() => topicPost, topicPost);

            restClient.Post(Operations.Topics, topicPost);
        }

        public Topic[] GetTopicsByGroup(Identity groupId)
        {
            return restClient.Get<Topic[]>(Operations.GetTopicsByGroup(groupId));
        }

        public void UpdateTopic(TopicPut topicPut)
        {
            Guard.Instance.ArgumentNotNull(() => topicPut, topicPut);

            restClient.Put(Operations.Topics, topicPut);
        }

        public void DeleteTopic(Identity topicId)
        {
            restClient.Delete(Operations.DeleteTopic(topicId));
        }

        #endregion

        #region Topic Groups

        public void CreateGroup(Group group)
        {
            Guard.Instance.ArgumentNotNull(() => group, group);
            if(group.IsPersisted)
            {
                throw new InvalidOperationException("The group is already persisted.");
            }
            var location = restClient.Post(Operations.Groups, new GroupPost
            {
                Name = group.Name,
                Description = group.Description
            });
            var createdGroup = restClient.GetFromUrl<Facade.Group>(location);
            group.Id = createdGroup.Id.ToString();
        }

        public Group[] GetGroups()
        {
            return restClient.Get<Group[]>(Operations.Groups);
        }

        public void UpdateGroup(GroupPut groupPut)
        {
            Guard.Instance.ArgumentNotNull(() => groupPut, groupPut);

            restClient.Put(Operations.Groups, groupPut);
        }

        public void DeleteGroup(Identity groupId)
        {
            restClient.Delete(Operations.DeleteGroup(groupId));
        }

        #endregion

        #region Subscriptions

        public void CreateSubscription(SubscriptionPost post)
        {
            Guard.Instance.ArgumentNotNull(() => post, post);

            restClient.Post(Operations.Subscriptions, post);
        }

        public Subscription[] GetSubscriptions()
        {
            return restClient.Get<Subscription[]>(Operations.Subscriptions);
        }

        public Subscription[] GetSubscriptionsByTopic(Identity topicId)
        {
            return restClient.Get<Subscription[]>(Operations.Subscriptions + "/topic/" + topicId);
        }

        public Subscription[] GetSubscriptionsByGroup(Identity groupId)
        {
            return restClient.Get<Subscription[]>(Operations.Subscriptions + "/topicgroup/" + groupId);
        }

        public void UpdateSubscription(SubscriptionPut put)
        {
            Guard.Instance.ArgumentNotNull(() => put, put);

            restClient.Put(Operations.Subscriptions, put);
        }

        public void DeleteSubscription(Identity subscriptionId)
        {
            restClient.Delete(Operations.DeleteSubscription(subscriptionId));
        }

        #endregion

        #region Messages

        public Uri PostMessage(Message message)
        {
            Guard.Instance
                .ArgumentNotNull(() => message, message)
                .ArgumentNotNull(() => message.TopicId, message.TopicId);

            var headers = message.Headers ?? new Header[0];

            return restClient.Post(Operations.PostMessagesOnTopic(message.TopicId), message.Payload, headers);
        }

        #region GetMessagesLink

        public Link[] GetMessagesLink(Identity subscriptionId)
        {
            return restClient.Get<Link[]>(Operations.GetMessagesBySubscription(subscriptionId));  
          
        }

        #endregion

        public HttpWebResponse GetMessage(string href)
        {
            Guard.Instance.ArgumentNotNullOrEmpty(() => href, href);

            return restClient.GetResponse(href);
        }

        public HttpWebResponse GetMessage(Identity topicId, Identity messageId)
        {
            return restClient.GetResponse(Operations.GetMessageInTopic(topicId, messageId));
        }

        #endregion                                                        
    }
}