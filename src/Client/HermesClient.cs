using System;
using System.Linq;
using TellagoStudios.Hermes.Client.Util;
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

        #endregion




    }
}