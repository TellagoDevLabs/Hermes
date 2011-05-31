using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace DataAccess.Tests.Repository
{
    class MongoDbGroupFixture : MongoDbBaseFixture
    {
        private IGroupRepository repository;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            repository = new MongoDbGroupRepository(connectionString);
        }

        [Test]
        public void Create_a_group()
        {
            var group = new Group
                              {
                                  Description = Utils.RandomString(),
                                  Name = Utils.RandomString()
                              };

            var result = repository.Create(group);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Id);
            Assert.AreEqual(group.Name, result.Name);
            Assert.AreEqual(group.Description, result.Description);
        }
    }
}
