using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.Business.Topics;
using TellagoStudios.Hermes.Business.Topics.Commands;

namespace Business.Tests.Topics
{
    [TestFixture]
    public class DeleteTopicCommandTests 
    {
        public IDeleteTopicCommand CreateCommand(IEntityById queryEntityById = null, 
                                                ICudOperations<Topic> cud = null) 
        {
            return new DeleteTopicCommand(
                                queryEntityById ?? Mock.Of<IEntityById>(), 
                                cud ?? Mock.Of<ICudOperations<Topic>>());    
        }

        [Test]
        public void WhenIdIsNull_ThenThrowIdMustNotBeNull()
        {
            var command = CreateCommand();
            command.Executing(c => c.Execute(new Topic {Id = null}))
                .Throws<ValidationException>()
                .And.Exception.Message.Should().Be.EqualTo(Messages.IdMustNotBeNull);
        }

        [Test]
        public void WhenTopicDoesNotExist_ThenThrowEntityNotFound()
        {
            var id = Identity.Random();
            var command = CreateCommand(Mock.Of<IEntityById>(q => q.Exist<Topic>(id) == false));

            command.Executing(c => c.Execute(new Topic {Id = id}))
                .Throws<EntityNotFoundException>();
        }

        [Test]
        public void WhenTopicExists_ThenDelete()
        {
            var id = Identity.Random();
            var repository = new StubCudOperations<Topic>(new Topic { Id = id });
            var command = CreateCommand(Mock.Of<IEntityById>(q => q.Exist<Topic>(id)==true),
                                        repository);
              
            command.Execute(new Topic { Id = id });
                
            repository.Documents.Should().Be.Empty();
        }
    }
}