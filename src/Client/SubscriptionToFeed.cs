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
        private readonly Facade.Topic topic;
        private readonly RestClient restClient;
        private readonly ISubject<string> subject;
        private string lastRead;
        private Stack<Tuple<string, string>> stack = new Stack<Tuple<string, string>>();

        public SubscriptionToFeed(Facade.Topic topic, RestClient restClient, int seconds = 10)
        {
            this.topic = topic;
            this.restClient = restClient;
            subject = new Subject<string>();
            Observable.Timer(TimeSpan.FromSeconds(seconds))
                .Subscribe(u => FetchFeed(topic.GetLinkForRelation("Current Feed")));
        }

        public void FetchFeed(string feedUrl)
        {

            var stream = restClient.GetStream(feedUrl);
            var formatter = new Atom10FeedFormatter();
            using (var xmlReader = XmlReader.Create(stream))
            {
                formatter.ReadFrom(xmlReader);
                var feed = formatter.Feed;
                var entriesToPush = feed.Items
                    .Where(item => item.Id != lastRead);

                foreach (var entry in entriesToPush)
                {
                    
                    var textSyndicationContent = entry.Content as TextSyndicationContent;
                    if(textSyndicationContent != null)
                    {
                        stack.Push(Tuple.Create(entry.Id, textSyndicationContent.Text));
                    }
                }

                if (!feed.Items.Any()|| feed.Items.Last().Id != lastRead)
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