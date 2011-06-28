using DataAccess.Tests.Util;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class TopicsSortedByNameTests : Repository.MongoDbBaseFixture
    {
        private Topic topic2;
        private Topic topic1;
        private Topic topic3;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            var group = new Group {Description = "Abcd", Name = "hello"};
            mongoDb.GetCollectionByType<Group>().Insert(group);
            
            topic1 = new Topic { Name = "Zos", GroupId = group.Id.Value, Description = "bbb" };
            topic2 = new Topic { Name = "Bos", GroupId = group.Id.Value, Description = "aaa" };
            topic3 = new Topic { Name = "Acd", GroupId = group.Id.Value, Description = "aaa" };
            mongoDb.GetCollectionByType<Topic>().InsertMany(topic1, topic2, topic3);
        }
        

        [Test]
        public void ShouldWork()
        {
            new TopicsSortedByName(connectionString)
                .Execute().Should().Have.SameSequenceAs(topic3, topic2, topic1);

        }
    }
}