using System;
using System.Linq;
using System.Net;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public class HermesClient
    {
        private readonly RestClient restClient;

        #region Constructors

        public HermesClient(string hermesAddress)
            : this(new Uri(hermesAddress), null)
        { }


        public HermesClient(string hermesAddress, string proxy)
            : this(new Uri(hermesAddress), proxy)
        {}

        public HermesClient(Uri hermesAddress, string proxy)
        {
            Guard.Instance.ArgumentNotNull(() => hermesAddress, hermesAddress);
            restClient = proxy != null ? new RestClient(hermesAddress, new Uri(proxy)) : new RestClient(hermesAddress);
        }

        #endregion

        #region Topic Groups

        public Group CreateGroup(string name, string description)
        {
            Guard.Instance.ArgumentNotNullOrEmpty(() => name, name);

            var location = restClient.Post(Operations.Groups, new GroupPost
            {
                Name = name,
                Description = description
            });
            var createdGroup = restClient.GetFromUrl<Facade.Group>(location);

            return new Group(createdGroup, restClient);
        }

        public Group CreateGroup(string name)
        {
            return CreateGroup(name, string.Empty);
        }

        public Group[] GetGroups()
        {
            return restClient.Get<Facade.Group[]>(Operations.Groups)
                            .Select(g => new Group(g, restClient))
                            .ToArray();
        }

        public void UpdateGroup(GroupPut groupPut)
        {
            Guard.Instance.ArgumentNotNull(() => groupPut, groupPut);

            restClient.Put(Operations.Groups, groupPut);
        }

        public void DeleteGroup(string groupId)
        {
            restClient.Delete(Operations.DeleteGroup((Identity) groupId));
        }

        #endregion


        //#region Topic
        //public void CreateTopic(Topic topic)
        //{
        //    Guard.Instance.ArgumentNotNull(() => topic, topic);
        //    //if (!topic.Group.IsPersisted)
        //    //{
        //    //    CreateGroup(topic.Group);
        //    //}

        //    var location = restClient.Post(Operations.Topics, new TopicPost
        //    {
        //        Name = topic.Name,
        //        Description = topic.Description,
        //        GroupId = (Identity)topic.Group.Id
        //    });

        //    var createdTopic = restClient.GetFromUrl<Facade.Topic>(location);
        //    topic.Id = createdTopic.Id.ToString();
        //}

        //public Topic[] GetTopicsByGroup(string groupId)
        //{
        //    return restClient.Get<Topic[]>(Operations.GetTopicsByGroup((Identity) groupId));
        //}

        //public void UpdateTopic(TopicPut topicPut)
        //{
        //    Guard.Instance.ArgumentNotNull(() => topicPut, topicPut);

        //    restClient.Put(Operations.Topics, topicPut);
        //}

        //public void DeleteTopic(string topicId)
        //{
        //    restClient.Delete(Operations.DeleteTopic((Identity) topicId));
        //} 
        //#endregion



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