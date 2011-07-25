using System;
using System.Linq;
using DataAccess.Tests.Repository;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class MessageKeysByGroupTests : MongoDbBaseFixture
    {

        private Identity[] groupIds = new Identity[4];
        private Identity[] topicIds = new Identity[4];
        private Identity[] msgIds = new Identity[4];

        private IChildGroupsOfGroup childGroupsOfGroup;
        private ITopicsByGroup topicsByGroup;

        [SetUp]
        public void Setup()
        {
            var colG = base.mongoDb.GetCollection<Group>(MongoDbConstants.GetCollectionNameForType<Group>());
            var colT = base.mongoDb.GetCollection<Topic>(MongoDbConstants.GetCollectionNameForType<Topic>());

            for (var i = 0; i<4; i++)
            {
                // Add group
                var group = new Group
                                {
                                    Name = "Group Foo " + i,
                                    ParentId = (i == 1 || i == 2) ? groupIds[i - 1] : new Identity?() // Set group hierarchy
                                };
                colG.Save(group);
                groupIds[i] = group.Id.Value;

                // Add a topic to the group
                var topic = new Topic {Name = "Topic foo " + i, GroupId = group.Id.Value};
                colT.Save(topic);
                topicIds[i] = topic.Id.Value;

                // Add a message to the topic
                var colM = base.mongoDb.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(topicIds[i]));
                var message = new Message
                                  {
                                      UtcReceivedOn = DateTime.UtcNow,
                                      TopicId = topicIds[i],
                                      Payload = new byte[] {1, 2, 3, 4, 5}
                                  };
                colM.Save(message);
                msgIds[i] = message.Id.Value;
            }

            childGroupsOfGroup = Mock.Of<IChildGroupsOfGroup>(q =>
                q.GetChildrenIds(groupIds[0]) == groupIds.Skip(1).Take(1) &&
                q.GetChildrenIds(groupIds[1]) == groupIds.Skip(2).Take(1) &&
                q.GetChildrenIds(groupIds[2]) == new Identity[0] &&
                q.GetChildrenIds(groupIds[3]) == new Identity[0]);

            topicsByGroup = Mock.Of<ITopicsByGroup>(q =>
                q.GetTopicIds(groupIds[0], null, null) == topicIds.Skip(0).Take(1) &&
                q.GetTopicIds(groupIds[1], null, null) == topicIds.Skip(1).Take(1) &&
                q.GetTopicIds(groupIds[2], null, null) == topicIds.Skip(2).Take(1) &&
                q.GetTopicIds(groupIds[3], null, null) == topicIds.Skip(3).Take(1));
        }

        [Test]
        public void WhenGroupHasNotParentButChildren_MustReturnAllGroupDescendentsMessage()
        {
            var query = CreateQuery(childGroupsOfGroup, topicsByGroup );
            var result = query.Get(groupIds[0])
                .ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
            Assert.IsTrue(result.Any(mk => mk.TopicId == topicIds[0] && mk.MessageId == msgIds[0]));
            Assert.IsTrue(result.Any(mk => mk.TopicId == topicIds[1] && mk.MessageId == msgIds[1]));
            Assert.IsTrue(result.Any(mk => mk.TopicId == topicIds[2] && mk.MessageId == msgIds[2]));

            Assert.IsTrue(!result.Any(mk => mk.TopicId == topicIds[3] && mk.MessageId == msgIds[3]));
        }

        [Test]
        public void WhenGroupHasAParentAndChildren_MustNotReturnParentMessages()
        {
            var query = CreateQuery(childGroupsOfGroup, topicsByGroup);
            var result = query.Get(groupIds[1])
                .ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.Any(mk => mk.TopicId == topicIds[1] && mk.MessageId == msgIds[1]));
            Assert.IsTrue(result.Any(mk => mk.TopicId == topicIds[2] && mk.MessageId == msgIds[2]));

            Assert.IsTrue(!result.Any(mk => mk.TopicId == topicIds[3] && mk.MessageId == msgIds[3]));
            Assert.IsTrue(!result.Any(mk => mk.TopicId == topicIds[0] && mk.MessageId == msgIds[0]));
        }


        [Test]
        public void WhenGroupHasParentButChildren_MustReturnItsMessagesOnly()
        {
            var query = CreateQuery(childGroupsOfGroup, topicsByGroup);
            var result = query.Get(groupIds[2])
                .ToArray();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.IsTrue(result.Any(mk => mk.TopicId == topicIds[2] && mk.MessageId == msgIds[2]));
        
            Assert.IsTrue(!result.Any(mk => mk.TopicId == topicIds[3] && mk.MessageId == msgIds[3]));
            Assert.IsTrue(!result.Any(mk => mk.TopicId == topicIds[0] && mk.MessageId == msgIds[0]));
            Assert.IsTrue(!result.Any(mk => mk.TopicId == topicIds[1] && mk.MessageId == msgIds[1]));
        }

        private MessageKeysByGroup CreateQuery(IChildGroupsOfGroup childGroupsOfGroup, ITopicsByGroup topicsByGroup)
        {
            return new MessageKeysByGroup(base.connectionString,
                                          childGroupsOfGroup ?? Mock.Of<IChildGroupsOfGroup>(),
                                          topicsByGroup ?? Mock.Of<ITopicsByGroup>());
        }
    }
}