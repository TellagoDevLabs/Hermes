using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Client.Util;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public class Group : ModelBase
    {
        private readonly Facade.Group @group;
        private readonly RestClient restClient;

        internal Group(Facade.Group group, RestClient restClient)
            : base((string) group.Id)
        {
            this.@group = @group;
            this.restClient = restClient;
        }

        public string Name
        {
            get { return group.Name; }
            set { group.Name = value; }
        }

        public string Description
        {
            get { return group.Description; }
            set { group.Description = value; }
        }

        public Topic CreateTopic(string name, string description = "")
        {
            var topicPost = new TopicPost {Name = name, Description = description, GroupId = (Identity)Id};
            var location = restClient.Post(group.GetLinkForRelation("Create Topic"), topicPost);
            var topicCreated = restClient.Get<Facade.Topic>(location.ToString());

            return new Topic(topicCreated, this, restClient);
        }

        public IEnumerable<Topic> GetTopics()
        {
            var topics = restClient.Get<Facade.Topic[]>(group.GetLinkForRelation("All Topics"));
            return topics.Select(tf => new Topic(tf, this, restClient)).ToList();
        }

        public void Delete()
        {
            restClient.Delete(group.GetLinkForRelation("Delete"));   
        }

        public void SaveChanges()
        {
            restClient.Put(group.GetLinkForRelation("Update"), 
                new GroupPut
                {
                    Description = Description,
                    Name = Name,
                    Id = (Identity) Id
                });
        }

        public Topic TryCreateTopic(string name, string description = "")
        {
            var topic = restClient.Get<Facade.Topic>(Operations.GetTopic(name, group.Id));
            return topic==null ?
                CreateTopic(name, description) : 
                new Topic( topic , restClient);
        }
    }
}