using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Topics;

namespace Business.Tests.Topics
{
    [TestFixture]
    public class DeleteTopicCommandTests 
    {
        public IDeleteTopicCommand CreateCommand(IEntityById queryEntityById = null, 
                                                IRepository<Topic> cud = null) 
        {
            return new DeleteTopicCommand(
                                queryEntityById ?? Mock.Of<IEntityById>(), 
                                cud ?? Mock.Of<IRepository<Topic>>());    
        }

        [Test]
        public void WhenTopicDoesNotExist_ThenThrowEntityNotFound()
        {
            var id = Identity.Random();
            var command = CreateCommand(Mock.Of<IEntityById>(q => q.Exist<Topic>(id) == false));

            command.Executing(c => c.Execute(id))
                .Throws<EntityNotFoundException>();
        }

        [Test]
        public void WhenTopicExists_ThenDelete()
        {
            var id = Identity.Random();
            var repository = new StubRepository<Topic>(new Topic { Id = id });
            var command = CreateCommand(Mock.Of<IEntityById>(q => q.Exist<Topic>(id)==true),
                                        repository);
              
            command.Execute(id);
                
            repository.Documents.Should().Be.Empty();
        }
    }
}