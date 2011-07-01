using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.Syndication;
using System.Xml;

namespace TellagoStudios.Hermes.Client
{
    public class SubscriptionToFeed : IObservable<string>
    {
        private readonly RestClient restClient;
        private readonly ISubject<string> subject;
        private string lastRead;
        private readonly Stack<Tuple<string, string>> stack 
                = new Stack<Tuple<string, string>>();
        private readonly object lck = new object();

        public SubscriptionToFeed(Facade.Topic topic, RestClient restClient, int seconds = 10)
        {
            this.restClient = restClient;
            subject = new Subject<string>();
            Observable.Timer(TimeSpan.FromSeconds(seconds))
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
                    subject.OnNext(entry.Item2);    
                }
            }
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}