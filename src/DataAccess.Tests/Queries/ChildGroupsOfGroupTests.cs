using DataAccess.Tests.Repository;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class ChildGroupsOfGroupTests : MongoDbBaseFixture
    {
        private Group withChilds;
        private Group withoutChilds;

        [SetUp]
        public void SetUp()
        {
            var collection = mongoDb.GetCollection(MongoDbConstants.Collections.Groups);
            collection.RemoveAll();

            withChilds = new Group{Name = "With childs"};
            collection.Insert(withChilds);

            withoutChilds = new Group { Name = "Without childs" , ParentId = withChilds.Id};
            collection.Insert(withoutChilds);
        }

        [Test]
        public void WhenGroupHasChilds_ThenHasChildsREturnsTrue()
        {
            var query = new ChildGroupsOfGroup(base.connectionString);
            query.HasChilds(withChilds).Should().Be.True();
        }

        [Test]
        public void WhenGroupHasNotChilds_ThenHasChildsREturnsFalse()
        {
            var query = new ChildGroupsOfGroup(base.connectionString);
            query.HasChilds(withoutChilds).Should().Be.False();
        }
    }
}
