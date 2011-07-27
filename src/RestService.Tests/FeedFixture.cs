using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Autofac;
using Moq;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Resources;

namespace RestService.Tests
{
    [TestFixture]
    public class FeedFixture : ResourceBaseFixture
    {
        private Mock<IGetWorkingFeedForTopic> getWorkingFeedForTopic;
        private Mock<IEntityById> entityById;
        private Mock<IMessageByMessageKey> messageByMessageKey;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            getWorkingFeedForTopic = new Mock<IGetWorkingFeedForTopic>();
            entityById = new Mock<IEntityById>();
            messageByMessageKey = new Mock<IMessageByMessageKey>();

            builder.RegisterInstance(new FeedResource(
                                         getWorkingFeedForTopic.Object,
                                         entityById.Object,
                                         messageByMessageKey.Object));
        }

        protected override Type GetServiceType()
        {
            return typeof(FeedResource);
        }

        protected override RestClient.SerializationType GetSerializationType()
        {
            return RestClient.SerializationType.Xml;
        }

        private SyndicationFeed GetFeedForMessage(Message message)
        {
            var messageId = message.Id.Value;
            var topicId = message.TopicId;

            var feedEntry = new FeedEntry() { MessageId = messageId, TimeStamp = DateTime.UtcNow };
            var feed = new Feed()
            {
                Entries = new List<FeedEntry>(new[] { feedEntry }),
                Id = Identity.Random(),
                TopicId = topicId,
                Updated = DateTime.UtcNow
            };

            var topic = new Topic()
            {
                Id = topicId,
                Name = "Topic Name",
                Description = "Topic Description",
                GroupId = Identity.Random()
            };

            var key = new MessageKey { MessageId = messageId, TopicId = topicId };

            messageByMessageKey
                .Setup(r => r.Get(It.Is<MessageKey>(k => k.TopicId == key.TopicId && k.MessageId == key.MessageId)))
                .Returns(message);

            getWorkingFeedForTopic
                .Setup(r => r.Execute(topicId))
                .Returns(feed);

            entityById
                .Setup(r => r.Get<Topic>(topicId))
                .Returns(topic);

            var client = new HttpClient(baseUri);

            var response = client.Get(topicId.ToString());
            var formatter = new Atom10FeedFormatter();

            using (var rdr = XmlReader.Create(response.Content.ContentReadStream))
            {
                formatter.ReadFrom(rdr);
                return formatter.Feed;
            }

        }

        [Test]
        public void Text_message_with_charset_is_included_as_text()
        {
            const string contentType = "text/plain;charset=UTF-8";
            const string payload = "Hello!";
            var messageId = Identity.Random();
            var topicId = Identity.Random();
            var message = new Message()
            {
                Id = messageId,
                TopicId = topicId,
                Payload = Encoding.ASCII.GetBytes(payload),
                UtcReceivedOn = DateTime.UtcNow
            };
            message.Headers.Add("Content-Type", new[] { contentType });

            var feed = GetFeedForMessage(message);

            var entry = feed.Items.Single();

            Assert.AreEqual(contentType, entry.Content.Type);
        }

        [Test]
        public void Nontext_message_is_linked_with_the_correct_content_type()
        {
            const string contentType = "application/json";
            const string payload = @"{""Hello"":""World!""}";
            var messageId = Identity.Random();
            var topicId = Identity.Random();
            var message = new Message()
            {
                Id = messageId,
                TopicId = topicId,
                Payload = Encoding.ASCII.GetBytes(payload),
                UtcReceivedOn = DateTime.UtcNow
            };
            message.Headers.Add("Content-Type", new[] { contentType });

            var feed = GetFeedForMessage(message);

            var entry = feed.Items.Single();

            Assert.AreEqual(contentType, entry.Content.Type);
        }

    }
}