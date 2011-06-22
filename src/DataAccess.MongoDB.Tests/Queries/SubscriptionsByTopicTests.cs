using DataAccess.Tests.Repository;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class SubscriptionsByTopicTests : MongoDbBaseFixture
    {
        private Topic tc;
        private Topic tc2;
        private Subscription subs1, subs2;


        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            var topics = mongoDb.GetCollectionByType<Topic>();
            var subscriptions = mongoDb.GetCollectionByType<Subscription>();
            var groups = mongoDb.GetCollectionByType<Group>();

            var group = new Group();
            groups.Insert(group);

            tc =  new Topic { Description = "T1", Name = "Aaaa", GroupId = group.Id.Value };
            tc2 = new Topic { Description = "T2", Name = "Aaaa", GroupId = group.Id.Value };
            topics.Insert(tc);
            topics.Insert(tc2);

            subs1 = new Subscription 
                            { 
                                TargetKind = TargetKind.Topic, 
                                TargetId = tc.Id , 
                                Callback = new Callback()
                            };

            subs2 = new Subscription
                            {
                                TargetKind = TargetKind.Topic,
                                TargetId = tc2.Id,
                                Callback = new Callback ()
                            };
            subscriptions.InsertBatch(new[] { subs1, subs2 });
        }

        [Test]
        public void ShouldWork()
        {
            var query = new SubscriptionsByTopic(base.connectionString);
            query.Execute(tc.Id.Value)
                .Should().Contain(subs1)
                .And.Not.Contain(subs2);
        }
    }
}