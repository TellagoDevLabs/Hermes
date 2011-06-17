using System;
using DataAccess.Tests.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.Queries;

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

            existTopicByTopicName.Execute("Foo", topic.Id).Should().Be.False();
        }

        [Test]
        public void WhenThereIsATopicWithSameNameAndDifferentId_ThenReturnsTrue()
        {
            var existTopicByTopicName = new ExistTopicByName(connectionString);
            var topic = new Topic { Id = Identity.Random(12), Name = "Foo", GroupId = Identity.Random(12) };
            topicsCollection.Insert(topic);
            existTopicByTopicName.Execute("Foo", Identity.Random(12)).Should().Be.True();
        }

        [Test]
        public void WhenThereIsATopicWithSameNameAndIdNull_ThenReturnsTrue()
        {
            var existTopicByTopicName = new ExistTopicByName(connectionString);
            var topic = new Topic { Id = Identity.Random(12), Name = "Foo", GroupId = Identity.Random(12) };
            topicsCollection.Insert(topic);
            existTopicByTopicName.Execute("Foo", null).Should().Be.True();
        }

        [Test]
        public void WhenThereNotIsATopicWithGivenName_ThenReturnsFalse()
        {
            var existTopicByTopicName = new ExistTopicByName(connectionString);
            topicsCollection.Insert(new Topic {Id = Identity.Random(12), Name = "Foo", GroupId = Identity.Random(12) });
            existTopicByTopicName.Execute("Bar").Should().Be.False();
        }
    }
}