using System;
using DataAccess.Tests.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class ExistTopicByNameTests : MongoDbBaseFixture
    {
        private MongoCollection<BsonDocument> topicsCollection;
        [SetUp]
        public void SetUp()
        {
            topicsCollection =  mongoDb.GetCollection(MongoDbConstants.Collections.Topics);
            topicsCollection.RemoveAll();
        }

        [Test]
        public void WhenThereIsATopicWithSameNameAndId_ThenReturnsFalse()
        {
            var existTopicByTopicName = new ExistTopicByName(connectionString);
            var topic = new Topic {Id = Identity.Random(12), Name = "Foo", GroupId = Identity.Random(12)};
            topicsCollection.Insert(topic);

            existTopicByTopicName.Execute(topic.GroupId, "Foo", topic.Id).Should().Be.False();
        }

        [Test]
        public void WhenThereIsATopicWithSameNameAndDifferentId_ThenReturnsTrue()
        {
            var existTopicByTopicName = new ExistTopicByName(connectionString);
            var topic = new Topic { Id = Identity.Random(12), Name = "Foo", GroupId = Identity.Random(12) };
            topicsCollection.Insert(topic);
            existTopicByTopicName.Execute(topic.GroupId, "Foo", Identity.Random(12)).Should().Be.True();
        }

        [Test]
        public void WhenThereIsATopicWithSameNameAndIdNull_ThenReturnsTrue()
        {
            var existTopicByTopicName = new ExistTopicByName(connectionString);
            var topic = new Topic { Id = Identity.Random(12), Name = "Foo", GroupId = Identity.Random(12) };
            topicsCollection.Insert(topic);
            existTopicByTopicName.Execute(topic.GroupId, "Foo", null).Should().Be.True();
        }

        [Test]
        public void WhenThereIsNotATopicWithGivenName_ThenReturnsFalse()
        {
            var existTopicByTopicName = new ExistTopicByName(connectionString);
            var topic = new Topic {Id = Identity.Random(12), Name = "Foo", GroupId = Identity.Random(12) };
            topicsCollection.Insert(topic);
            existTopicByTopicName.Execute(topic.GroupId, "Bar").Should().Be.False();
        }

        [Test]
        public void WhenThereIsATopicWithSameNameInDifferentGroup_ThenReturnsFalse()
        {
            var existTopicByTopicName = new ExistTopicByName(connectionString);
            var topic = new Topic { Id = Identity.Random(12), Name = "Foo", GroupId = Identity.Random(12) };
            topicsCollection.Insert(topic);
            existTopicByTopicName.Execute(Identity.Random(12), "Foo", Identity.Random(12)).Should().Be.False();
        }
    }
}