using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.Syndication;
using System.Xml;
using TellagoStudios.Hermes.Client.Serialization;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace TellagoStudios.Hermes.Client
{
    public class SubscriptionToFeed : SubscriptionToFeed<string>
    {
        public SubscriptionToFeed(Facade.Topic topic, RestClient restClient, TimeSpan interval, IScheduler scheduler = null)
            : base(topic, restClient, interval, scheduler)
        {
        }
    }
 
    public class SubscriptionToFeed<T> : IObservable<T>
    {
        private readonly RestClient restClient;
        private readonly ISubject<T> subject;
        private string lastRead;
        private readonly Stack<Tuple<string, Uri, string>> stack = new Stack<Tuple<string, Uri, string>>();
        private readonly object lck = new object();

        public SubscriptionToFeed(Facade.Topic topic, RestClient restClient, TimeSpan interval, IScheduler scheduler = null)
        {
            this.restClient = restClient;
            subject = new Subject<T>();
            Observable.Timer(interval, scheduler ?? Scheduler.TaskPool)
                .Repeat()
                .Subscribe(u =>
                               {
                                   lock(lck)
                                   {
                                       FetchFeed(topic.GetLinkForRelation("Current Feed"));
                                   }
                               });
        }

        public void FetchFeed(string feedUrl)
        {

            var stream = restClient.GetStream(feedUrl);
            var formatter = new Atom10FeedFormatter();
            using (var xmlReader = XmlReader.Create(stream))
            {
                formatter.ReadFrom(xmlReader);
                var feed = formatter.Feed;
                var entriesToPush = feed.Items;
                bool foundLastOne = false;
                foreach (var entry in entriesToPush)
                {
                    if(entry.Id == lastRead)
                    {
                        foundLastOne = true;
                        break;
                    }
                    var urlSyndicationContent = entry.Content as UrlSyndicationContent;
                    if(urlSyndicationContent != null)
                    {
                        stack.Push(Tuple.Create(entry.Id, urlSyndicationContent.Url, urlSyndicationContent.Type));
                    }
                }

                if (lastRead == null || !foundLastOne)
                {
                    var prev = feed.Links.FirstOrDefault(l => l.RelationshipType == "prev-archive");
                    if(prev != null)
                    {
                        FetchFeed(prev.Uri.ToString());    
                    }
                }

                while (stack.Count > 0)
                {
                    var entry = stack.Pop();
                    lastRead = entry.Item1;
                    T t;
                    using (var streamT = restClient.GetStream(entry.Item2))
                    {
                        t = Serializer.Instance.Deserialize<T>(entry.Item3, streamT);
                    }
                    subject.OnNext(t);
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}