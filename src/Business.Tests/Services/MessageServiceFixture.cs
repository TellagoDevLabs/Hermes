using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.Business.Validator;
using Moq;
using NUnit.Framework;

namespace Business.Tests.Services
{
    [TestFixture]
    public class MessageServiceFixture
    {
        private MessageService service;
        private Mock<IMessageRepository> mockedRepository;
        private Mock<ITopicService> mockedTopicService;
        //private Mock<ISubscriptionService> mockedSubscriptionService;

        //[TestFixtureSetUp]
        //public void SetUpFixture()
        //{
        //    mockedTopicService = new Mock<ITopicService>(MockBehavior.Loose);
        //    mockedRepository = new Mock<IMessageRepository>(MockBehavior.Loose);
        //    mockedSubscriptionService = new Mock<ISubscriptionService>(MockBehavior.Loose);

        //    service = new MessageService
        //    {
        //        Repository = mockedRepository.Object,
        //        Validator = new MessageValidator
        //        {
        //            TopicService = mockedTopicService.Object, 
        //            SubscriptionService = mockedSubscriptionService.Object
        //        },
        //        TopicService = mockedTopicService.Object,
        //        SubscriptionService = mockedSubscriptionService.Object
        //    };
        //}

        [Test]
        public void should_create_a_message()
        {
            var headers = new Dictionary<string, string[]>();
            headers.Add("single", new[] {"singleValue"});
            headers.Add("empty", new string[] { });
            headers.Add("many", new[] { "value1", "value2", "value3" });

            var promotedProperties = new Dictionary<string, string>();
            promotedProperties.Add("null", null);
            promotedProperties.Add("empty", string.Empty);
            promotedProperties.Add("string", "\"single string value\"");
            promotedProperties.Add("integer", "20");
            promotedProperties.Add("entity", "{\"prop\":20}");

            var message = new Message
                              {
                                  Payload = new byte[] {1, 2, 3, 4, 5},
                                  TopicId = Identity.Random(12),
                                  Headers = headers,
                                  PromotedProperties = promotedProperties,
                                  UtcReceivedOn = DateTime.UtcNow
                              };

            var response = Clone(message);
            response.Id = Identity.Random(12);

            mockedTopicService.Setup(ts => ts.Exists(message.TopicId))
                .Returns(true);
            
            mockedTopicService.Setup(ts => ts.Get(message.TopicId))
                .Returns(new Topic { Id = message.TopicId });
            
            //mockedSubscriptionService.Setup(ss => ss.Find(It.Is<string>(query => query != null), null, null))
            //    .Returns(new Subscription[0]);
            
            mockedRepository.Setup(r => r.Create(message)).Returns(response);


            var result = service.Create(message);

            mockedRepository.Verify(r => r.Create(message));

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, response.Id);
        }

        private Message Clone(Message message)
        {
            if (message == null) return null;

            var m = new Message
                        {
                            Id = message.Id,
                            TopicId = message.TopicId,
                            UtcReceivedOn = message.UtcReceivedOn
                        };

            if (message.Payload == null)
            {
                m.Payload = null;
            }
            else
            {
                var len = message.Payload.Length;
                m.Payload = new byte[len];
                if (len > 0)
                {
                    message.Payload.CopyTo(m.Payload, 0);
                }
            }

            message.Headers
                .ForEach(h =>
                             {
                                 if (h.Value == null)
                                 {
                                     m.Headers.Add(h.Key, h.Value);
                                 }
                                 else
                                 {
                                     var len = h.Value.Length;
                                     var values = new string[len];
                                     h.Value.CopyTo(values, 0);
                                     m.Headers.Add(h.Key, values);
                                 }
                             });

            message.PromotedProperties
                .ForEach(pp => m.PromotedProperties.Add(pp.Key, pp.Value));

            return m;
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void should_throws_a_validation_on_create()
        {
            var headers = new Dictionary<string, string[]>();
            headers.Add("single", new[] { "singleValue" });
            headers.Add("empty", new string[] { });
            headers.Add("many", new[] { "value1", "value2", "value3" });

            var promotedProperties = new Dictionary<string, string>();
            promotedProperties.Add("null", null);
            promotedProperties.Add("empty", string.Empty);
            promotedProperties.Add("string", "\"single string value\"");
            promotedProperties.Add("integer", "20");
            promotedProperties.Add("entity", "{\"prop\":20}");

            var message = new Message
            {
                Payload = new byte[] { 1, 2, 3, 4, 5 },
                TopicId = Identity.Random(),
                Headers = headers,
                PromotedProperties = promotedProperties,
                UtcReceivedOn = DateTime.UtcNow
            };

            var response = Clone(message);
            response.Id = Identity.Random();

            mockedTopicService.Setup(ts => ts.Exists(message.TopicId)).Returns(false);
            mockedRepository.Setup(r => r.Create(message)).Returns(response);

            var result = service.Create(message);

            Assert.IsNull(result);
        }


        [Test]
        public void should_get_a_message()
        {
            var message = new Message
            {
                Id = Identity.Random(),
                TopicId = Identity.Random(),
                Payload = new byte[] { 1, 2, 3, 4, 5 }
            };

            var key = new MessageKey { TopicId = message.TopicId, MessageId = message.Id.Value };

            mockedRepository.Setup(r => r.Get(It.Is<MessageKey>(k => k.MessageId == message.Id && k.TopicId == message.TopicId))).Returns(message);
            mockedTopicService.Setup(ts => ts.Exists(message.TopicId)).Returns(true);

            var result = service.Get(key);

            mockedRepository.Verify(r => r.Get(key));

            Assert.IsNotNull(result);
            Assert.AreEqual(message.Id, result.Id);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void should_throws_a_validation_on_get()
        {
            var message = new Message
            {
                Id = Identity.Random(),
                TopicId = Identity.Random(),
                Payload = new byte[] { 1, 2, 3, 4, 5 }
            };

            var key = new MessageKey { TopicId = message.TopicId, MessageId = message.Id.Value };

            mockedRepository.Setup(r => r.Get(It.Is<MessageKey>(k => k.MessageId == message.Id && k.TopicId == message.TopicId))).Returns(message);
            mockedTopicService.Setup(ts => ts.Exists(message.TopicId)).Returns(false);

            var result = service.Get(key);
        }

        [Test]
        public void get_message_should_return_null_if_message_does_not_exist()
        {
            var message = new Message
            {
                Id = Identity.Random(),
                TopicId = Identity.Random(),
                Payload = new byte[] { 1, 2, 3, 4, 5 }
            };

            var key = new MessageKey { TopicId = message.TopicId, MessageId = message.Id.Value };

            mockedRepository.Setup(r => r.Get(It.Is<MessageKey>(k => k.MessageId == message.Id && k.TopicId == message.TopicId))).Returns((Message)null);
            mockedTopicService.Setup(ts => ts.Exists(message.TopicId)).Returns(true);

            var result = service.Get(key);

            mockedRepository.Verify(r => r.Get(key));

            Assert.IsNull(result);
        }
    }
}
