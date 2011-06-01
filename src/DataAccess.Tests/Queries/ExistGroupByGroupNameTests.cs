using DataAccess.Tests.Repository;
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
        [Test]
        public void WhenThereIsAGroupWithGivenName_ThenReturnsTrue()
        {
            var existGroupByGroupName = new ExistGroupByGroupName(connectionString);
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                        .Insert(new Group {Name = "Foo"});
            existGroupByGroupName.Execute("Foo").Should().Be.True();
        }

        [Test]
        public void WhenThereNotIsAGroupWithGivenName_ThenReturnsFalse()
        {
            var existGroupByGroupName = new ExistGroupByGroupName(connectionString);
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                        .Insert(new Group { Name = "Foo" });
            existGroupByGroupName.Execute("Bar").Should().Be.False();
        }
    }
}