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
    public class ExistEntityByIdTests : MongoDbBaseFixture
    {
        [Test]
        public void WhenIdExist_ThenReturnsTrue()
        {
            var existEntityById = new ExistEntityById(connectionString);
            var document = new Group { Name = "Foo" };
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                                .Insert(document);

            existEntityById.Execute<Group>(document.Id.Value)
                        .Should().Be.True();
            
        }
    }
    
}