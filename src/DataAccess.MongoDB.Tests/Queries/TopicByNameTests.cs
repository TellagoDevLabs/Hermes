using DataAccess.Tests.Repository;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class TopicByNameTests : MongoDbBaseFixture
    {
        Group group;
        Group groupOther;
        Topic topicInGroup;
        Topic topicInGroupOther;
        Topic topicWithoutGroup;

        [SetUp]
        public void SetUp()
        {
            var topicsCollection = base.mongoDb.GetCollection<Topic>(MongoDbConstants.Collections.Topics);
            var groupsCollection = base.mongoDb.GetCollection<Group>(MongoDbConstants.Collections.Groups);
            topicsCollection.RemoveAll();
            groupsCollection.RemoveAll();

            group = new Group { Name = "A" };
            groupsCollection.Save(group);

            groupOther = new Group { Name = "B" };
            groupsCollection.Save(groupOther);

            topicInGroup = new Topic() { Name = "Foo", GroupId = group.Id };
            topicsCollection.Save(topicInGroup);

            topicInGroupOther = new Topic() { Name = "Foo", GroupId = groupOther.Id };
            topicsCollection.Save(topicInGroupOther);

            topicWithoutGroup = new Topic() { Name = "Bar" };
            topicsCollection.Save(topicWithoutGroup);
                
        }

        [Test]
        public void Correct_topic_name_in_correct_group()
        {
            var query = new TopicByName(connectionString);
            query.Exists(topicInGroup.Name, group.Id).Should().Be.True();
            
            var result = query.Get(topicInGroup.Name, group.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(topicInGroup.Id, result.Id);
            Assert.AreEqual(topicInGroup.Name, result.Name);
            Assert.AreEqual(group.Id, result.GroupId);
        }

        [Test]
        public void Correct_topic_name_in_invalid_group()
        {
            var groupId = Identity.Random(12);
            var query = new TopicByName(connectionString);
            query.Exists(topicInGroup.Name, groupId).Should().Be.False();

            var result = query.Get(topicInGroup.Name, groupId);
            Assert.IsNull(result);
        }

        [Test]
        public void Correct_topic_name_without_group()
        {
            var query = new TopicByName(connectionString);
            query.Exists(topicWithoutGroup.Name, null).Should().Be.True();

            var result = query.Get(topicWithoutGroup.Name, null);
            Assert.IsNotNull(result);
            Assert.AreEqual(topicWithoutGroup.Id, result.Id);
            Assert.AreEqual(topicWithoutGroup.Name, result.Name);
            Assert.IsNull(result.GroupId);
        }

        [Test]
        public void Invalid_topic_name_with_group()
        {
            var name = "Invalid";
            var groupId = Identity.Random(12);
            var query = new TopicByName(connectionString);
            query.Exists(name, groupId).Should().Be.False();

            var result = query.Get(name, groupId);
            Assert.IsNull(result);
        }

        [Test]
        public void Invalid_topic_name_without_group()
        {
            var name = "Invalid";
            var query = new TopicByName(connectionString);
            query.Exists(name, null).Should().Be.False();

            var result = query.Get(name, null);
            Assert.IsNull(result);
        }
    }
}