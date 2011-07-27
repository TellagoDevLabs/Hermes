using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Serialization;

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
        private readonly Stack<Tuple<string, string>> stack = new Stack<Tuple<string, string>>();
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
                    var textSyndicationContent = entry.Content as TextSyndicationContent;
                    if(textSyndicationContent != null)
                    {
                        stack.Push(Tuple.Create(entry.Id, textSyndicationContent.Text));
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
                    subject.OnNext(Deserialize(entry.Item2));    
                }
            }
        }

        private static T Deserialize(string data)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)data;
            }

            using (var reader = new StringReader(data))
            {
                var serializer = new XmlSerializer(typeof (T));
                return (T) serializer.Deserialize(reader);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}