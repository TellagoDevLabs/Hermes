using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using NUnit.Framework;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace DataAccess.Tests.Repository
{
    class MongoDbMessageFixture : MongoDbBaseFixture
    {
        private IMessageRepository repository;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            repository = new MongoDbMessageRepository(connectionString);
        }

        [Test]
        public void Create_a_message()
        {
            var headers = new Dictionary<string, string[]>();
            headers.Add("single", new[] { "singleValue" });
            headers.Add("empty", new string[] { });
            headers.Add("many", new[] { "value1", "value2", "value3" });

            var promotedProperties = new Dictionary<string, string>();
            promotedProperties.Add("null", "null");
            promotedProperties.Add("empty", "\"\"");
            promotedProperties.Add("string", "\"single string value\"");
            promotedProperties.Add("integer", "20");
            promotedProperties.Add("array", "[20, \"sam string\"]");
            promotedProperties.Add("entity", "{ \"prop\" : 20 }");

            var message = new Message
                              {
                                  Payload = new byte[] { 1, 2, 3, 4, 5 },
                                  TopicId = Identity.Random(Utils.MongoObjectId),
                                  Headers = headers,
                                  PromotedProperties = promotedProperties,
                                  UtcReceivedOn = DateTime.UtcNow
                              };

            var result = repository.Create(message);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Id);
            Assert.AreEqual(message.Payload, result.Payload);
            Assert.AreEqual(message.UtcReceivedOn, result.UtcReceivedOn);
            Assert.AreEqual(message.TopicId, result.TopicId);

            Utils.AreEquivalent(message.Headers, result.Headers);
            Utils.AreEquivalent(message.PromotedProperties, result.PromotedProperties);
        }

        [Test]
        public void Get_MessageKeys_for_a_topic()
        {
            #region Populate DB


            var topic = new Topic { Id = Identity.Random(Utils.MongoObjectId) };
            var messages = new[] { 
                new Message { Id = Identity.Random(Utils.MongoObjectId), TopicId = topic.Id.Value },
                new Message { Id = Identity.Random(Utils.MongoObjectId), TopicId = topic.Id.Value },
                new Message { Id = Identity.Random(Utils.MongoObjectId), TopicId = topic.Id.Value }};

            var name = MongoDbConstants.GetCollectionNameForMessage(topic.Id.Value);
            if (mongoDb.CollectionExists(name))
            {
                mongoDb.DropCollection(name);
            }

            var col = mongoDb.GetCollection<Message>(name);
            Array.ForEach(messages, m => col.Save(m));

            #endregion

            // Validate without filter
            var keys = repository.GetMessageKeys(topic.Id.Value)
                .ToArray();
            Assert.IsNotNull(keys);
            Assert.AreEqual(messages.Length, keys.Count());
            Assert.IsTrue(messages.All(m => keys.Any(key => key.MessageId == m.Id.Value)));

            //TODO:
            // Validate with filter
            //keys = repository.GetMessageKeys(topic.Id.Value, "{\"Number\":{$gt:10}}")
            //    .ToArray();
            //Assert.IsNotNull(keys);
            //Assert.AreEqual(2, keys.Count());
        }

        [Test]
        public void Get_Message()
        {
            #region Populate DB

            var messageID = Identity.Random(Utils.MongoObjectId);
            var messagePayload = new byte[] { 1, 2, 3, 4, 5 };
            var messageHeaders = new Dictionary<string, string[]>();
            messageHeaders.Add("Content-Type", new [] {"text/text"});
            var messagePP = new Dictionary<string, string>();
            messagePP.Add("Amount", "100");
            var messageReceivedOn = DateTime.UtcNow;

            var topicId = Identity.Random(Utils.MongoObjectId);
            var messages = new[] { 
                new Message { Id = Identity.Random(Utils.MongoObjectId), TopicId = topicId, Payload = new  byte[] { 1, 2, 3 }, UtcReceivedOn = DateTime.UtcNow },
                new Message { Id = messageID, TopicId = topicId, Payload = messagePayload, UtcReceivedOn = messageReceivedOn, Headers = messageHeaders, PromotedProperties = messagePP },
                new Message { Id = Identity.Random(Utils.MongoObjectId), TopicId = topicId, Payload = new  byte[] { 1, 2, 3 }, UtcReceivedOn = DateTime.UtcNow }
            };

            var name = MongoDbConstants.GetCollectionNameForMessage(topicId);
            var topic = new Topic { Id = topicId };
            if (mongoDb.CollectionExists(name))
            {
                mongoDb.DropCollection(name);
            }

            var col = mongoDb.GetCollection<Message>(name);
            Array.ForEach(messages, m => col.Save(m));
            
            #endregion

            var key = new MessageKey { MessageId = messageID, TopicId = topicId };
            var message = repository.Get(key);

            Assert.IsNotNull(message);
            Assert.AreEqual(messageID, message.Id);
            Assert.AreEqual(topic.Id, message.TopicId);
        }

        [Test]
        public void Get_Message_with_an_ID_that_does_not_exist()
        {
            #region Populate DB

            var messageID = Identity.Random(Utils.MongoObjectId);
            var topicId = Identity.Random(Utils.MongoObjectId);
            var topic = new Topic { Id = topicId };
            var messages = new[] { 
                new Message { Id = Identity.Random(Utils.MongoObjectId), TopicId = topicId },
                new Message { Id = Identity.Random(Utils.MongoObjectId), TopicId = topicId },
                new Message { Id = Identity.Random(Utils.MongoObjectId), TopicId = topicId }};

            var name = MongoDbConstants.GetCollectionNameForMessage(topicId);
            if (mongoDb.CollectionExists(name))
            {
                mongoDb.DropCollection(name);
            }

            var col = mongoDb.GetCollection<Message>(name);
            Array.ForEach(messages, m => col.Save(m));

            #endregion

            var key = new MessageKey { MessageId = messageID, TopicId = topicId };
            var message = repository.Get(key);

            Assert.IsNull(message);
        }
    }
}
