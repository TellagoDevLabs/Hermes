using System;
using System.Configuration;
using System.IO;
using System.Linq;
using TellagoStudios.Hermes.Client.Util;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public class HermesClient
    {
        private readonly RestClient restClient;

        #region Constructors

        /// <summary>
        /// This constructor will try to get
        /// the Hermes's address from an appSetting named hermes
        /// </summary>
        public HermesClient()
            : this (ConfigurationManager.AppSettings["hermes"])
        {}

        public HermesClient(string hermesAddress)
            : this(new Uri(hermesAddress), null)
        {}


        public HermesClient(string hermesAddress, string proxy)
            : this(new Uri(hermesAddress), proxy)
        {}

        public HermesClient(Uri hermesAddress, string proxy)
        {
            Guard.Instance.ArgumentNotNull(() => hermesAddress, hermesAddress);
            restClient = proxy != null ? new RestClient(hermesAddress, new Uri(proxy)) : new RestClient(hermesAddress);
        }

        #endregion

        public T GetMessage<T>(Uri url)
        {
            return restClient.Get<T>(url);
        }

        public Stream GetMessageAsStream(Uri url)
        {
            return restClient.GetStream(url);
        }

        public Uri TryPostMessage<T>(string groupName, string topicName, T data)
        {
           var location = TryCreateGroup(groupName)
                .TryCreateTopic(topicName)
                .PostMessage(data);

            return new Uri(location);
        }

        #region Topic

        public Topic CreateTopicWithoutGroup(string name, string description = "")
        {
            var topicPost = new TopicPost { Name = name, Description = description };
            var location = restClient.Post(Operations.Topics, topicPost);
            var topic = restClient.Get<Facade.Topic>(location.ToString());

            return topic == null ? null : new Topic(topic, restClient);
        }

        public Topic GetTopicWithoutGroup(string name)
        {
            var topic = restClient.Get<Facade.Topic>(Operations.GetTopic(name));
            return topic == null ? null : new Topic(topic, restClient);
        }

        #endregion

        #region Groups

        public Group CreateGroup(string name, string description)
        {
            Guard.Instance.ArgumentNotNullOrEmpty(() => name, name);

            var location = restClient.Post(Operations.Groups, new GroupPost
            {
                Name = name,
                Description = description
            });

            if (location==null) return null;

            var createdGroup = restClient.Get<Facade.Group>(location);
            return new Group(createdGroup, restClient);
        }

        public Group CreateGroup(string name)
        {
            return CreateGroup(name, string.Empty);
        }

        public Group GetGroupByName(string name)
        {
            var g = restClient.Get<Facade.Group>(Operations.GetGroup(name));
            return g == null ? null : new Group(g, restClient);
        }

        public Group GetGroup(string id)
        {
            var g = restClient.Get<Facade.Group>(Operations.GetGroup((Identity) id));
            return g == null ? null : new Group(g, restClient);
        }

        public Group[] GetGroups()
        {
            return restClient.Get<Facade.Group[]>(Operations.Groups)
                            .Select(g => new Group(g, restClient))
                            .ToArray();
        }

        public Group TryCreateGroup(string name)
        {
            var group = GetGroupByName(name);
            return group ?? CreateGroup(name);
        }

        public Group TryCreateGroup(string name, string description)
        {
            var group = GetGroups().FirstOrDefault(g => g.Name == name);
            return group ?? CreateGroup(name);
        }

        #endregion
    }
}