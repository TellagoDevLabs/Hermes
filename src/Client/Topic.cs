using System;
using System.IO;
using System.Text;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
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
                                   Description = Description,
                                   Name = Name,
                                   GroupId = (Identity) group.Id,
                                   Id = (Identity) Id
                               };
            restClient.Put(topic.GetLinkForRelation("Update"), topicPut);
        }

        /// <summary>
        /// Post a new message and return the id
        /// </summary>
        /// <returns></returns>
        public string PostMessage<T>(T data)
        {
            var location = restClient.Post(topic.GetLinkForRelation("Post Message"), data);
            return location.ToString();
        }

        public string PostMessage(MemoryStream data, string contentType)
        {
            var location = restClient.Post(topic.GetLinkForRelation("Post Message"), data,contentType: contentType);
            return location.ToString();
        }

        public string PostStringMessage(string message)
        {
            var data = new MemoryStream(Encoding.UTF8.GetBytes(message));
            return PostMessage(data, "text/plain");
        }

        /// <summary>
        /// Poll the feed of recent events.
        /// </summary>
        /// <param name="interval">polling interval in seconds</param>
        /// <returns>an observable sequence of messages</returns>
        public IObservable<string> PollFeed(int interval = 10)
        {
            return PollFeed(TimeSpan.FromSeconds(interval));
        }

        /// <summary>
        /// Poll the feed of recent events.
        /// </summary>
        /// <param name="timeSpan">polling interval</param>
        /// <returns>an observable sequence of messages</returns>
        public IObservable<string> PollFeed(TimeSpan timeSpan)
        {
            return new SubscriptionToFeed(topic, restClient, timeSpan);
        }
    }
}