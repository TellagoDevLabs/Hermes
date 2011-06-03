using DataAccess.Tests.Repository;
using MongoDB.Bson;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.Queries;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class CudGroupsTests : MongoDbBaseFixture
    {
        [Test]
        public void InsertingShouldWork()
        {
            var cudGroup = new CudOperations<Group>(connectionString);
            var document = new Group{Name = "Test", Description = "Abcd"};
            cudGroup.MakePersistent(document);

            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                .FindOneById(document.Id.Value.ToBson())
                .Should().Not.Be.Null();
        }


        [Test]
        public void DeleteShouldWork()
        {
            var cudGroup = new CudOperations<Group>(connectionString);

            var document = new Group { Name = "Test", Description = "Abcd" };
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups).Insert(document);

            //act
            cudGroup.MakeTransient(document);

            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                .FindOneById(document.Id.Value.ToBson())
                .Should().Be.Null();
        }

        [Test]
        public void UpdateShouldWork()
        {
            var cudGroup = new CudOperations<Group>(connectionString);

            var document = new Group { Name = "Test", Description = "Abcd" };
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups).Insert(document);

            document.Name = "Tito";
            //act
            cudGroup.Update(document);

            mongoDb.GetCollection<Group>(MongoDbConstants.Collections.Groups)
                .FindOneById(document.Id.Value.ToBson())
                .Name.Should().Be.EqualTo("Tito");
        }
    }
}