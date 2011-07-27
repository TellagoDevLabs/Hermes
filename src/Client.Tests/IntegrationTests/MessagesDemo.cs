using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Client.Tests.Util;

namespace TellagoStudios.Hermes.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class MessagesDemo : IntegrationTestBase
    {
        private readonly HermesClient client = new HermesClient("http://localhost:40403");
        private Topic topic;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            topic = client.CreateGroup("Group1")
                          .CreateTopic("Topic1");
        }

        #endregion

        [Test]
        public void CanPostMessageToTopic()
        {
            var messageId = topic.PostMessage(new MemoryStream(Encoding.UTF8.GetBytes("Hello world")));
            messageId.Should().Not.Be.NullOrEmpty();
        }

        [Test]
        public void CanPostAndGetTypedMessageFromTopic()
        {
            var news = new News { Title = "Hello world", Date = DateTime.Now };
            var messageId = topic.PostMessage(news);
            messageId.Should().Not.Be.NullOrEmpty();
            var url = new Uri(messageId);
            var news2 = client.GetMessage<News>(url);
            news2.Satisfy(n => news.Title == n.Title && news.Date == n.Date);
        }

        [Test]
        public void CanSubscribeToTopicFeed()
        {
            var read = new List<string>();
            using (topic.PollFeed(1)
                        .ObserveOn(Scheduler.CurrentThread)
                        .Subscribe(read.Add))
            {
                topic.PostStringMessage("a");
                topic.PostStringMessage("b");
                topic.PostStringMessage("c");
                while (read.Count < 3) { }
                read.Should().Have.SameSequenceAs("a", "b", "c");                
            }
        }

        [Test]
        public void CanSubscribeToTypedTopicFeed()
        {
            var news = new []
            { 
                new News {Title = "One", Date = DateTime.Now },
                new News {Title = "Two", Date = DateTime.Now.AddMinutes(1) },
                new News {Title = "Three", Date = DateTime.Now.AddMinutes(2) }
            };
            
            var topicNews = client.TryCreateGroup("Group1")
                          .TryCreateTopic("News");

            foreach (var n in news)
            {
                topicNews.PostMessage(n);
            }

            var read = new List<News>();
            using (topicNews.PollFeed<News>(1)
                        .ObserveOn(Scheduler.CurrentThread)
                        .Subscribe(read.Add))
            {
                while (read.Count < 3) { }
                read.Should().Have.SameSequenceAs(news);
            }
        }

        [Test]
        public void CanSubscribeToTopicFeedAndReceiveMoreThan15MEssagesInOrder()
        {
            var read = new List<string>();
            var messages = Enumerable.Range(0, 20)
                .Select(m => m.ToString()).ToList();
            using (topic.PollFeed(1)
                        .ObserveOn(Scheduler.NewThread)
                        .Subscribe(read.Add))
            {
                messages.ForEach(m => topic.PostStringMessage(m));
                
                Thread.Sleep(TimeSpan.FromSeconds(5));

                read.Should().Have.SameSequenceAs(messages);
            }
        }

        [Test]
        public void Samples()
        {
            
        }
    }
}