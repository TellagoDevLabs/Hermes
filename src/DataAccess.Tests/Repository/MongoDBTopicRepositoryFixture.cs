using System;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Repository;

namespace DataAccess.Tests.Repository
{
    public class MongoDBTopicRepositoryFixture :  MongoDbBaseFixture
    {
        private ITopicRepository repository;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            repository = new MongoDbTopicRepository(connectionString);
        }

        [Test]
        public void Create_a_topic()
        {
            var groupId = CreateGroup();
            var topic = new Topic
                            {
                                Name = "sample",
                                Description = "Description",
                                GroupId = groupId
                            };
            var result = repository.Create(topic);

            Assert.IsNotNull(result.Id);
            Assert.That(result.Name, Is.EqualTo(topic.Name));
            Assert.That(result.Description, Is.EqualTo(topic.Description));
            Assert.That(result.GroupId, Is.EqualTo(topic.GroupId));
        }

        #region Private methods

        private string RandomString()
        {
            return Guid.NewGuid().ToString();
        }

        private Identity CreateGroup()
        {

            var g = new Group
            {
                Name = RandomString(),
                Description = RandomString()
            };

            var collection = mongoDb.GetCollection<Group>(MongoDbConstants.Collections.Groups);
            collection.Save(g);
            Assert.IsNotNull(g.Id);
            return g.Id.Value;
        }
        #endregion
    }
}
