using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using Autofac;
using Moq;
using NUnit.Framework;
using System.Net;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.RestService;
using System.Collections.Generic;

namespace RestService.Tests
{
    [TestFixture]
    public class MessageFixture : ResourceBaseFixture
    {
        private Mock<IMessageService> mockedService;


        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedService = new Mock<IMessageService>(MockBehavior.Loose);
            builder.RegisterInstance(new MessageResource(mockedService.Object));
        }

        protected override Type GetServiceType()
        {
            return typeof(MessageResource);
        }

        [Test]
        public void Post_of_a_valid_message()
        {
            var client = new HttpClient(baseUri);
            var content = new StringContent("sample");
            content.Headers.Add(Constants.PrivateHeaders.PromotedProperty + "String", "\"valueOne\"");
            content.Headers.Add(Constants.PrivateHeaders.PromotedProperty + "Integer", "100");
            content.Headers.Add(Constants.PrivateHeaders.PromotedProperty + "Document", "{\"prop1\":\"value1\", \"prop2\":2}");

            content.Headers.Add(Constants.PrivateHeaders.PromotedProperties,
                                "str=\"strValue\",int=200,doc={\"prop\":10}");

            var topicId = Identity.Random();
            var response = new Message {Id = Identity.Random() };

            mockedService.Setup(s => s.Create(It.Is<Message>(m => m != null && m.TopicId == topicId))).Returns(response);

            var httpResponse = client.Post(baseUri+"/topic/" + topicId, content);

            var contentStr = httpResponse.Content.ReadAsString();
        }

        [Test]
        public void Should_get_a_messagge()
        {
            var contentType = "text/text";
            var ppName = "Amount";
            var ppValue = 100;
            var payload = "Hello!";
            var message = new Message()
            {
                Id = Identity.Random(),
                TopicId = Identity.Random(),
                Payload = Encoding.ASCII.GetBytes(payload),
                UtcReceivedOn = DateTime.UtcNow
            };

            message.Headers.Add("Content-Type", new[] { contentType });
            message.PromotedProperties.Add(ppName, ppValue.ToString());

            var key = new MessageKey { MessageId = message.Id.Value, TopicId = message.TopicId };

            mockedService.Setup(r => r.Get(It.Is<MessageKey>(k => k.TopicId == key.TopicId && k.MessageId == key.MessageId))).Returns(message);

            var client = new HttpClient(baseUri);
            var url = baseUri + message.Id.Value.ToString() + "/topic/" + message.TopicId;
            var result = client.Get(url);

            mockedService.Verify(r => r.Get(It.Is<MessageKey>(k => k.TopicId == key.TopicId && k.MessageId == key.MessageId)));

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(payload, result.Content.ReadAsString());
            Assert.AreEqual(contentType, result.Content.Headers.ContentType.MediaType);
        }

        [Test]
        public void Validates_a_get_with_invalid_data()
        {
            mockedService.Setup(s => s.Get(It.IsAny<MessageKey>())).Throws<ValidationException>();

            var client = new HttpClient(baseUri);
            var url = baseUri.ToString() + Identity.Random() + "/topic/" + Identity.Random();
            var result = client.Get(url);

            mockedService.Verify(r => r.Get(It.IsAny<MessageKey>()));

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void Validates_a_get_of_a_message_that_does_not_exist()
        {
            mockedService.Setup(s => s.Get(It.IsAny<MessageKey>())).Returns((Message)null);

            var client = new HttpClient(baseUri);
            var url = baseUri.ToString() + Identity.Random() + "/topic/" + Identity.Random();
            var result = client.Get(url);

            mockedService.Verify(r => r.Get(It.IsAny<MessageKey>()));

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
