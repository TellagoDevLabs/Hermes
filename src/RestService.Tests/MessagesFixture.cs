using System;
using System.Net.Http;
using Autofac;
using Moq;
using NUnit.Framework;
using System.Net;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Messages;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Resources;

namespace RestService.Tests
{
    [TestFixture]
    public class MessagesFixture : ResourceBaseFixture
    {
        private Mock<ICreateMessageCommand> createMessageCommand;
        private Mock<IMessageKeysByTopic> messageKeysByTopic;
        private Mock<IMessageKeysByGroup> messageKeysByGroup;
        private Mock<IMessageKeysBySubscription> messageKeysBySubscription;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            createMessageCommand = new Mock<ICreateMessageCommand> ();
            messageKeysByTopic = new Mock<IMessageKeysByTopic>();
            messageKeysByGroup = new Mock<IMessageKeysByGroup>();
            messageKeysBySubscription = new Mock<IMessageKeysBySubscription>();

            builder.RegisterInstance(new MessagesResource(
                createMessageCommand.Object,
                messageKeysByTopic.Object,
                messageKeysByGroup.Object,
                messageKeysBySubscription.Object));
        }

        protected override RestClient.SerializationType GetSerializationType()
        {
            return RestClient.SerializationType.Xml;
        }

        protected override Type GetServiceType()
        {
            return typeof(MessagesResource);
        }

        [Test]
        public void Post_of_a_valid_message()
        {
            var client = new HttpClient(baseUri);
            var content = new StringContent("sample");

            var topicId = Identity.Random();
            var response = new Message {Id = Identity.Random() };

            createMessageCommand
                .Setup(s => s.Execute(It.Is<Message>(m => m != null && m.TopicId == topicId)))
                .Callback<Message>(m => m.Id = Identity.Random()); ;

            var httpResponse = client.Post(baseUri+"/topic/" + topicId, content);

            var contentStr = httpResponse.Content.ReadAsString();
        }

        [Test] 
        public void Should_get_a_messagge_by_topic()
        {
            var key = new MessageKey {MessageId = Identity.Random(), TopicId = Identity.Random()};

            messageKeysByTopic
                .Setup(r => r.Get(key.TopicId, null, null, null))
                .Returns(new[] {key});

            var client = new HttpClient(baseUri);
            var url = baseUri + "topic/" + key.TopicId;
            var result = client.Get(url);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void Should_get_a_messagge_by_topic_using_last()
        {
            var last = Identity.Random();
            var key = new MessageKey { MessageId = Identity.Random(), TopicId = Identity.Random() };

            messageKeysByTopic
                .Setup(r => r.Get(key.TopicId, last, null, null))
                .Returns(new [] { key });

            var client = new HttpClient(baseUri);
            var url = baseUri + "topic/" + key.TopicId + "?last=" + last;
            var result = client.Get(url);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
