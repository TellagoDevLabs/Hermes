using System;
using DataAccess.Tests.Repository;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class QueryEntityByIdTests : MongoDbBaseFixture
    {
        [Test]
        public void WhenIdExist_ThenReturnsTrue()
        {
            var existEntityById = new EntityById(connectionString);
            var document = new Group { Name = "Foo" };
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                                .Insert(document);

            existEntityById.Exist<Group>(document.Id.Value)
                        .Should().Be.True();
            
        }


        [Test]
        public void WhenEntityExist_ThenGetReturnsEntity()
        {
            var existEntityById = new EntityById(connectionString);
            var document = new Group { Name = "Foo" };
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                                .Insert(document);

            existEntityById.Get<Group>(document.Id.Value)
                .Satisfy(e => e != null && e.Name == "Foo");

        }

        [Test]
        public void WhenDoesNotIdExist_ThenReturnsFalse()
        {
            var existEntityById = new EntityById(connectionString);
            var document = new Group { Name = "Foo" };

            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                                .Insert(document);

            existEntityById.Exist<Group>(new Identity("4de7e38617b6c420a45a84c4"))
                        .Should().Be.False();

        }
    }
    
}