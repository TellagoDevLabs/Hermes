using DataAccess.Tests.Repository;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class SubscriptionsByGroupTests : MongoDbBaseFixture
    {
        private Group gr, gr2;
        private Subscription subs1, subs2;
        

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            var groups = mongoDb.GetCollectionByType<Group>();
            var subscriptions = mongoDb.GetCollectionByType<Subscription>();

            gr = new Group {Name = "Grupo!"};
            gr2 = new Group { Name = "Grupo!" };
            groups.InsertBatch(new[] {gr, gr2});

            subs1 = new Subscription { TargetKind = TargetKind.Group, TargetId = gr.Id  , Callback = new Callback() };
            subs2 = new Subscription { TargetKind = TargetKind.Group, TargetId = gr2.Id, Callback = new Callback() };
            subscriptions.InsertBatch(new[] {subs1, subs2});
        }

        [Test]
        public void ShouldWork()
        {
            var query = new SubscriptionsByGroup(base.connectionString);
            query.Execute(gr.Id.Value)
                .Should().Contain(subs1)
                .And.Not.Contain(subs2);
        }         
    }
}