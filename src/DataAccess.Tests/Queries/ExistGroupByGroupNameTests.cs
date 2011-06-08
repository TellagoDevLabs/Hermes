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
    public class ExistGroupByGroupNameTests : MongoDbBaseFixture
    {
        private MongoCollection<BsonDocument> groupsCollection;
        [SetUp]
        public void SetUp()
        {
            groupsCollection =  mongoDb.GetCollection(MongoDbConstants.Collections.Groups);
            groupsCollection.RemoveAll();
        }

        [Test]
        public void WhenThereIsAGroupWithSameNameAndId_ThenReturnsFalse()
        {
            var existGroupByGroupName = new ExistGroupByGroupName(connectionString);
            var entity = new Group { Id = Identity.Random(12), Name = "Foo" };
            groupsCollection.Insert(entity);

            existGroupByGroupName.Execute("Foo", entity.Id).Should().Be.False();
        }

        [Test]
        public void WhenThereIsAGroupWithSameNameAndDifferentId_ThenReturnsTrue()
        {
            var existGroupByGroupName = new ExistGroupByGroupName(connectionString);
            var entity = new Group {Id = Identity.Random(12), Name = "Foo" };
            groupsCollection.Insert(entity);
            existGroupByGroupName.Execute("Foo", new Identity("4de7e38617b6c420a45a84c4")).Should().Be.True();
        }

        [Test]
        public void WhenThereIsAGroupWithSameNameAndIdNull_ThenReturnsTrue()
        {
            var existGroupByGroupName = new ExistGroupByGroupName(connectionString);
            var entity = new Group { Id = Identity.Random(12),  Name = "Foo" };
            groupsCollection.Insert(entity);
            existGroupByGroupName.Execute("Foo", null).Should().Be.True();
        }

        [Test]
        public void WhenThereNotIsAGroupWithGivenName_ThenReturnsFalse()
        {
            var existGroupByGroupName = new ExistGroupByGroupName(connectionString);
            groupsCollection.Insert(new Group { Id = Identity.Random(12), Name = "Foo" });
            existGroupByGroupName.Execute("Bar").Should().Be.False();
        }
    }
}