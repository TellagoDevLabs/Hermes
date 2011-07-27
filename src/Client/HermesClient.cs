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
            return restClient.GetFromUrl<T>(url);
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

        #region Topic Groups

        public Group CreateGroup(string name, string description)
        {
            Guard.Instance.ArgumentNotNullOrEmpty(() => name, name);

            var location = restClient.Post(Operations.Groups, new GroupPost
            {
                Name = name,
                Description = description
            });
            var createdGroup = restClient.Get<Facade.Group>(location.ToString());

            return new Group(createdGroup, restClient);
        }

        public Group CreateGroup(string name)
        {
            return CreateGroup(name, string.Empty);
        }

        public Group GetGroup(string id)
        {
            var g = restClient.Get<Facade.Group>(Operations.GetGroup((Identity) id));
            return new Group(g, restClient);
        }

        public Group[] GetGroups()
        {
            return restClient.Get<Facade.Group[]>(Operations.Groups)
                            .Select(g => new Group(g, restClient))
                            .ToArray();
        }

        public Group TryCreateGroup(string name)
        {
            var group = GetGroups().FirstOrDefault(g => g.Name == name);
            return group ?? CreateGroup(name);
        }

        public Group TryCreateGroup(string name, string description)
        {
            var group = GetGroups().FirstOrDefault(g => g.Name == name);
            if (group != null) return group;
            return CreateGroup(name, description);
        }

        #endregion
    }
}