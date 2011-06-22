using DataAccess.Tests.Repository;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class TopicsByGroupTests : MongoDbBaseFixture
    {
        private Group groupWithTopics;
        private Group groupWithoutTopics;

        [SetUp]
        public void SetUp()
        {
            var topicsCollection = base.mongoDb.GetCollection<Topic>(MongoDbConstants.Collections.Topics);
            var groupsCollection = base.mongoDb.GetCollection<Group>(MongoDbConstants.Collections.Groups);
            topicsCollection.RemoveAll();
            groupsCollection.RemoveAll();

            groupWithTopics = new Group { Id = Identity.Random(12), Name = "With topics" };
            groupsCollection.Insert(groupWithTopics);

            groupWithoutTopics = new Group { Id = Identity.Random(12), Name = "With topics" };
            groupsCollection.Insert(groupWithoutTopics);

            var topic = new Topic {Name = "Topic 1", GroupId = groupWithTopics.Id.Value};
            topicsCollection.Insert(topic);
        }


        [Test]
        public void WhenGroupHasTopic_HasTopicReturnsTrue()
        {
            var query = new TopicsByGroup(connectionString);
            query.HasTopics(groupWithTopics.Id.Value).Should().Be.True();
        }

        [Test]
        public void WhenGroupHasTopic_HasTopicReturnsFalse()
        {
            var query = new TopicsByGroup(connectionString);
            query.HasTopics(groupWithoutTopics.Id.Value).Should().Be.False();
        }
        
    }
}