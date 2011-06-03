using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace Business.Tests.Groups
{
    [TestFixture]
    public class DeleteGroupCommandTests 
    {
        public DeleteGroupCommand CreateCommand(IQueryEntityById queryEntityById = null, 
                                                ICudOperations<Group> cudGroup = null, 
                                                IQueryChildGroups queryChildGroups = null)
        {
            return new DeleteGroupCommand(
                                queryEntityById ?? Mock.Of<IQueryEntityById>(), 
                                cudGroup ?? Mock.Of<ICudOperations<Group>>(),
                                queryChildGroups ?? Mock.Of<IQueryChildGroups>());    
        }

        [Test]
        public void WhenIdIsNull_ThenThrowIdMustNotBeNull()
        {
            var command = CreateCommand();
            command.Executing(c => c.Execute(new Group {Id = null}))
                .Throws<ValidationException>()
                .And.Exception.Message.Should().Be.EqualTo(Messages.IdMustNotBeNull);
        }

        [Test]
        public void WhenGroupDoesNotExist_ThenThrowEntityNotFound()
        {
            var command = CreateCommand(Mock.Of<IQueryEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == false));

            command.Executing(c => c.Execute(new Group {Id = new Identity("4de7e38617b6c420a45a84c4")}))
                .Throws<EntityNotFoundException>();
        }

        [Test]
        public void WhenGroupHasChildGroups_ThenThrow()
        {
            var command = CreateCommand(queryEntityById: Mock.Of<IQueryEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == true),
                                        queryChildGroups: Mock.Of<IQueryChildGroups>(qg => qg.HasChilds(It.IsAny<Group>()) == true));

            command.Executing(c => c.Execute(new Group { Id = new Identity("4de7e38617b6c420a45a84c4") }))
                .Throws<ValidationException>()
                .And.Exception.Message.Should().Be.EqualTo(Messages.GroupContainsChildGroups);
        }

        [Test]
        public void WhenGroupExists_ThenDelete()
        {
            var repository = new StubCudOperations<Group>(new Group { Id = new Identity("4de7e38617b6c420a45a84c4") });
            var command = CreateCommand(Mock.Of<IQueryEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == true),
                                        repository);
              
            command.Execute(new Group { Id = new Identity("4de7e38617b6c420a45a84c4") });
                
            repository.Documents.Should().Be.Empty();
        }
    }
}