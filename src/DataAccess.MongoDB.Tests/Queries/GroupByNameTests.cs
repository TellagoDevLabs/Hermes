using DataAccess.Tests.Repository;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class GroupByNameTests : MongoDbBaseFixture
    {
        Group group;

        [SetUp]
        public void SetUp()
        {
            var groupsCollection = base.mongoDb.GetCollection<Group>(MongoDbConstants.Collections.Groups);
            groupsCollection.RemoveAll();

            group = new Group { Name = "A" };
            groupsCollection.Save(group);
        }

        [Test]
        public void Correct_group_name()
        {
            var query = new GroupByName(connectionString);
            query.Exists(group.Name).Should().Be.True();

            var result = query.Get(group.Name);
            Assert.IsNotNull(result);
            Assert.AreEqual(group.Id, result.Id);
            Assert.AreEqual(group.Name, result.Name);
        }

        [Test]
        public void Invalid_group_name()
        {
            var name = "invalid";
            var query = new GroupByName(connectionString);
            query.Exists(name).Should().Be.False();

            var result = query.Get(name);
            Assert.IsNull(result);
        }
    }
}