using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public class ModelBase
    {
        public ModelBase(string id)
        {
            Id = id;
        }

        public string Id { get; internal set; }
        
        public bool Equals(ModelBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ModelBase) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }

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

        public string Name { get { return group.Name; } }
        public string Description { get { return group.Description; } }

        public Topic CreateTopic(string name, string description = "")
        {
            var topicPost = new TopicPost {Name = name, Description = description, GroupId = (Identity) this.Id};
            var location = restClient.Post(group.GetLinkForRelation("Create Topic"), topicPost);
            var topicCreated = restClient.Get<Facade.Topic>(location.ToString());

            return new Topic(topicCreated, this, restClient);
        }

        public IEnumerable<Topic> GetAllTopics()
        {
            var topics = restClient.Get<Facade.Topic[]>(group.GetLinkForRelation("All Topics"));
            return topics.Select(tf => new Topic(tf, this, restClient)).ToList();
        }
    }


    public class Topic : ModelBase
    {
        private readonly Facade.Topic topic;
        private readonly Group @group;
        private readonly RestClient restClient;

        public Topic(
            Facade.Topic topic, 
            Group group,
            RestClient restClient)
            : base((string) topic.Id)
        {
            this.topic = topic;
            this.@group = @group;
            this.restClient = restClient;
        }


        public string Name
        {
            get { return topic.Name; }
            set { topic.Name = value; }
        }
        public string Description
        {
            get { return topic.Description; }
            set { topic.Description = value; }
        }
        public Group Group { get { return group; } }

        public void Delete()
        {
            restClient.Delete(topic.GetLinkForRelation("Delete"));
        }

        public void SaveChanges()
        {
            var topicPut = new TopicPut
                               {
                                   Description = this.Description,
                                   Name = this.Name,
                                   GroupId = (Identity) group.Id,
                                   Id = (Identity) Id
                               };
            restClient.Put(topic.GetLinkForRelation("Update"), topicPut);
        }
    }
}