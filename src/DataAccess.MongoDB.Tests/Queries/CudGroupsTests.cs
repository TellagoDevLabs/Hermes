using DataAccess.Tests.Repository;
using MongoDB.Bson;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Commands;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class CudGroupsTests : MongoDbBaseFixture
    {
        [Test]
        public void InsertingShouldWork()
        {
            var cudGroup = new Repository<Group>(connectionString);
            var entity = new Group{Name = "Test", Description = "Abcd"};
            cudGroup.MakePersistent(entity);

            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                .FindOneById(entity.Id.Value.ToBson())
                .Should().Not.Be.Null();
        }


        [Test]
        public void DeleteShouldWork()
        {
            var cudGroup = new Repository<Group>(connectionString);

            var entity = new Group { Name = "Test", Description = "Abcd" };
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups).Insert(entity);

            //act
            cudGroup.MakeTransient(entity.Id.Value);

            mongoDb.GetCollection(MongoDbConstants.Collections.Groups)
                .FindOneById(entity.Id.Value.ToBson())
                .Should().Be.Null();
        }

        [Test]
        public void UpdateShouldWork()
        {
            var cudGroup = new Repository<Group>(connectionString);

            var entity = new Group { Name = "Test", Description = "Abcd" };
            mongoDb.GetCollection(MongoDbConstants.Collections.Groups).Insert(entity);

            entity.Name = "Tito";
            //act
            cudGroup.Update(entity);

            mongoDb.GetCollection<Group>(MongoDbConstants.Collections.Groups)
                .FindOneById(entity.Id.Value.ToBson())
                .Name.Should().Be.EqualTo("Tito");
        }
    }
}