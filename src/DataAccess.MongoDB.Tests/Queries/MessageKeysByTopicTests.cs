using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Tests.Repository;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class MessageKeysByTopicTests : MongoDbBaseFixture
    {

        private Identity topicId;
        private Identity[] msgIds;

        [SetUp]
        public void Setup()
        {
            topicId = Identity.Random(12);
            var col = base.mongoDb.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(topicId));

            var ids = new List<Identity>();
            for (int i=0; i<3;i++)
            {
                var msg = new Message
                              {
                                  UtcReceivedOn = DateTime.UtcNow,
                                  TopicId = topicId,
                                  Payload = new byte[] {1, 2, 3, 4, 5}
                              };
                col.Save(msg);
                ids.Add(msg.Id.Value);
            }

            msgIds = ids.ToArray();
        }

        [Test]
        public void WhenMessageExists_MustReturnMessage()
        {
            var query = new MessageKeysByTopic(base.connectionString);
            var result = query.Get(topicId, null, null, null)
                .ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(msgIds.Length, result.Length);
        }

        [Test]
        public void ReturnsMessageUsingLast()
        {
            var query = new MessageKeysByTopic(base.connectionString);
            var result = query.Get(topicId, msgIds[1], null, null)
                .ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(msgIds[2], result[0].MessageId);
        }
    }
}