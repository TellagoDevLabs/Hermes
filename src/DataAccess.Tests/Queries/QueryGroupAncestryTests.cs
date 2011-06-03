using DataAccess.Tests.Repository;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.Queries;

namespace DataAccess.Tests.Queries
{
    public class QueryGroupAncestryTests : MongoDbBaseFixture
    {
        private Group foz;
        private Group bar;
        private Group foo;
        private Group baz;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            var groupCollection = base.mongoDb.GetCollection<Group>(MongoDbConstants.Collections.Groups);

            //foo => Bar => Baz
            //    => Foz
            foo = new Group {Name = "foo"};
            groupCollection.Insert(foo);
            bar = new Group {Name = "Bar", ParentId = foo.Id};
            groupCollection.Insert(bar);
            baz = new Group {Name = "Baz", ParentId = bar.Id};
            groupCollection.Insert(baz);
            foz = new Group {Name = "Foz", ParentId = bar.Id};
            groupCollection.Insert(foz);
        }
        [Test]
        public void WhenGroupHasNoAncestry_ThenResultShouldContainItSelf()
        {
            var query = new QueryGroupAncestry(base.connectionString);
            query.Execute(foo)
                .Should().Contain(foo);
        }

        [Test]
        public void WhenGroupHasAncestry_ThenResultShouldContainItSelf()
        {
            var query = new QueryGroupAncestry(base.connectionString);
            query.Execute(baz)
                .Should().Have.SameValuesAs(foo, bar, baz);
        }
    }
}