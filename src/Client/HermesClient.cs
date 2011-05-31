using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.RestService.Facade;

namespace TellagoStudios.Hermes.Client
{
    public class HermesClient : RestClient, IPubliser, ISuscriber, IAdmin
    {
        private static Func<string> _getUrl;

        public static Func<string> GetUrl
        {
            get { return _getUrl; }
            set { _getUrl = value; BaseAddress = new Uri(value()); }
        }

        #region Constructors
        internal HermesClient(string hermesAddress)
        {
            Guard.Instance.ArgumentNotNullOrEmpty(() => hermesAddress, hermesAddress);

            BaseAddress = new Uri(hermesAddress);
        }

        internal HermesClient(Uri hermesAddress)
        {
            Guard.Instance.ArgumentNotNull(() => hermesAddress, hermesAddress);

            BaseAddress = hermesAddress;
        }
        #endregion

        #region Factories
        public static IAdmin NewAdmin()
        {
            Guard.Instance.ArgumentNotNull(() => BaseAddress, BaseAddress);

            return new HermesClient(BaseAddress);
        }

        public static IPubliser NewPublisher()
        {
            Guard.Instance.ArgumentNotNull(() => BaseAddress, BaseAddress);

            return new HermesClient(BaseAddress);
        }

        public static ISuscriber NewSuscriber()
        {
            Guard.Instance.ArgumentNotNull(() => BaseAddress, BaseAddress);

            return new HermesClient(BaseAddress);
        }         
        #endregion

        #region Topics

        public void CreateTopic(TopicPost topicPost)
        {
            Guard.Instance.ArgumentNotNull(() => topicPost, topicPost);

            Post(Operations.Topics, topicPost);
        }

        public Topic[] GetTopicsByGroup(Identity groupId)
        {
            return Get<Topic[]>(Operations.GetTopicsByGroup(groupId));
        }

        public void UpdateTopic(TopicPut topicPut)
        {
            Guard.Instance.ArgumentNotNull(() => topicPut, topicPut);

            Put(Operations.Topics, topicPut);
        }

        public void DeleteTopic(Identity topicId)
        {
            Delete(Operations.DeleteTopic(topicId));
        }

        #endregion

        #region Topic Groups

        public void CreateGroup(GroupPost groupPost)
        {
            Guard.Instance.ArgumentNotNull(() => groupPost, groupPost);

            Post(Operations.Groups, groupPost);
        }

        public Group[] GetGroups()
        {
            return Get<Group[]>(Operations.Groups);
        }

        public void UpdateGroup(GroupPut groupPut)
        {
            Guard.Instance.ArgumentNotNull(() => groupPut, groupPut);

            Put(Operations.Groups, groupPut);
        }

        public void DeleteGroup(Identity groupId)
        {
            Delete(Operations.DeleteGroup(groupId));
        }

        #endregion

        #region Subscriptions

        public void CreateSubscription(SubscriptionPost post)
        {
            Guard.Instance.ArgumentNotNull(() => post, post);

            Post(Operations.Subscriptions, post);
        }

        public Subscription[] GetSubscriptions()
        {
            return Get<Subscription[]>(Operations.Subscriptions);
        }

        public Subscription[] GetSubscriptionsByTopic(Identity topicId)
        {
            return Get<Subscription[]>(Operations.Subscriptions + "/topic/" + topicId);
        }

        public Subscription[] GetSubscriptionsByGroup(Identity groupId)
        {
            return Get<Subscription[]>(Operations.Subscriptions + "/topicgroup/" + groupId);
        }

        public void UpdateSubscription(SubscriptionPut put)
        {
            Guard.Instance.ArgumentNotNull(() => put, put);

            Put(Operations.Subscriptions, put);
        }

        public void DeleteSubscription(Identity subscriptionId)
        {
            Delete(Operations.DeleteSubscription(subscriptionId));
        }

        #endregion

        #region Messages

        public Link PostMessage(Message message)
        {
            Guard.Instance
                .ArgumentNotNull(() => message, message)
                .ArgumentNotNull(() => message.TopicId, message.TopicId);

            var headers = (message.PromotedProperties ?? new Header[0])
                .Select(pp => new Header(Constants.PrivateHeaders.PromotedProperty + pp.Name, pp.Value))
                .Union(message.Headers ?? new Header[0]);

            var link = Post<Stream, Link>(Operations.PostMessagesOnTopic(message.TopicId), message.Payload, headers);
            return link;
        }

        #region GetMessagesLink

        public Link[] GetMessagesLink(Identity subscriptionId)
        {
            return Get<Link[]>(Operations.GetMessagesBySubscription(subscriptionId));  
          
        }

        #endregion

        public HttpWebResponse GetMessage(string href)
        {
            Guard.Instance.ArgumentNotNullOrEmpty(() => href, href);

            return GetResponse(href);
        }

        public HttpWebResponse GetMessage(Identity topicId, Identity messageId)
        {
            return GetResponse(Operations.GetMessageInTopic(topicId, messageId));
        }

        #endregion                                                        
    }
}