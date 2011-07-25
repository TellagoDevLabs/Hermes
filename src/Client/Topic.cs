using System;
using System.IO;
using System.Text;
using System.Linq;
using TellagoStudios.Hermes.Client.Util;
using TellagoStudios.Hermes.Facade;
using System.Reactive.Concurrency;

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

        public Uri GetLinkForFeed()
        {
            return new Uri(topic.GetLinkForRelation("Current Feed"));
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
        /// <param name="scheduler">Reactive framework's scheduler</param>
        /// <returns>an observable sequence of messages</returns>
        public IObservable<string> PollFeed(TimeSpan timeSpan, IScheduler scheduler = null)
        {
            return new SubscriptionToFeed(topic, restClient, timeSpan, scheduler);
        }

        public Uri[] PollMessages(Uri lastMessage = null)
        {
            return restClient.Get<Link[]>(Operations.GetMessagesByTopic(topic.Id, lastMessage))
                .Select(l => new Uri(l.Uri))
                .ToArray();
        }

        public IObservable<Message<T>> PollMessages<T>(TimeSpan timeSpan, Uri lastMessage , IScheduler scheduler = null)
            where T : class, new()
        {
            return new SubscriptionToTopic<T>(this, restClient, timeSpan, lastMessage, scheduler);
        }
    }
}