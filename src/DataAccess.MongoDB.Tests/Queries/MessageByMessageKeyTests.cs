using System;
using DataAccess.Tests.Repository;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class MessageByMessageKeyTests : MongoDbBaseFixture
    {
        private readonly Identity topicId = Identity.Random(12);

        [Test]
        public void WhenMessageExists_MustReturnMessage()
        {
            var col = base.mongoDb.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(topicId));
            var msg = new Message
                          {
                              UtcReceivedOn = DateTime.UtcNow,
                              TopicId = topicId,
                              Payload = new byte[] { 1, 2, 3, 4, 5 }
                          };
            col.Save(msg);

            Assert.IsNotNull(msg.Id);

            var key = new MessageKey { MessageId = msg.Id.Value, TopicId = topicId };
            var query = new MessageByMessageKey(base.connectionString);
            var result = query.Get(key);
            Assert.IsNotNull(result);
            Assert.AreEqual(msg.Id, result.Id);
            Assert.AreEqual(msg.UtcReceivedOn.Date, result.UtcReceivedOn.Date);
            Assert.AreEqual(msg.TopicId, result.TopicId);
            CollectionAssert.AreEqual(msg.Payload, result.Payload);
        }


        [Test]
        public void WhenMessageDoesNotExist_MustReturnNull()
        {
            var col = base.mongoDb.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(topicId));
            var msg = new Message
                          {
                              UtcReceivedOn = DateTime.UtcNow,
                              TopicId = topicId,
                              Payload = new byte[] { 1, 2, 3, 4, 5 }
                          };
            col.Save(msg);

            Assert.IsNotNull(msg.Id);

            var key = new MessageKey { MessageId = Identity.Random(12), TopicId = topicId };
            var query = new MessageByMessageKey(base.connectionString);
            var result = query.Get(key);
            Assert.IsNull(result);
        }
    }
}