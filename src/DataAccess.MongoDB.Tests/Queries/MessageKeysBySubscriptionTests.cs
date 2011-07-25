using System.Linq;
using DataAccess.Tests.Repository;
using Moq;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class MessageKeysBySubscriptionTests : MongoDbBaseFixture
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WhenExistASubscriptionToATopic_MustReturnTopicsMessages()
        {
            var topicId = Identity.Random();
            var keys = new[] { new MessageKey { MessageId = Identity.Random(), TopicId = topicId } };

            var subscriptionId = Identity.Random();
            var subscription = new Subscription { Id = subscriptionId, TargetId = topicId, TargetKind = TargetKind.Topic };


            var query = CreateQuery(Mock.Of<IEntityById>(q => q.Get<Subscription>(subscriptionId) == subscription),
                messageKeysByTopic: Mock.Of<IMessageKeysByTopic>(q => q.Get(topicId, null, null, null) == keys));

            var result = query.Get(subscriptionId)
                .ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void WhenExistASubscriptionToAGroup_MustReturnTopicsMessages()
        {
            var groupId = Identity.Random();
            var keys = new[] { new MessageKey { MessageId = Identity.Random(), TopicId = Identity.Random()} };

            var subscriptionId = Identity.Random();
            var subscription = new Subscription { Id = subscriptionId, TargetId = groupId, TargetKind = TargetKind.Group};


            var query = CreateQuery(Mock.Of<IEntityById>(q => q.Get<Subscription>(subscriptionId) == subscription),
                Mock.Of<IMessageKeysByGroup>(q => q.Get(groupId, null, null, null) == keys));

            var result = query.Get(subscriptionId)
                .ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
        }

        private MessageKeysBySubscription CreateQuery(
            IEntityById entityById = null,
            IMessageKeysByGroup messageKeysByGroup = null,
            IMessageKeysByTopic messageKeysByTopic = null)
        {
            return new MessageKeysBySubscription(base.connectionString,
                entityById ?? Mock.Of<IEntityById>(),
                messageKeysByTopic ?? Mock.Of<IMessageKeysByTopic>(),
                messageKeysByGroup ?? Mock.Of<IMessageKeysByGroup>());
        }
    }
}