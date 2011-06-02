using System;
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
    public class UpdateGroupCommandTests
    {
        [Test]
        public void WhenGroupNameIsDuplicated_ThenThrowValidateException()
        {
            var groupCommand = CreateUpdateGroupCommand(Mock.Of<IExistGroupByGroupName>(q => q.Execute("test", It.IsAny<Identity>()) == true));

            groupCommand.Executing(gc => gc.Execute(new Group { Name = "test", Id = new Identity("4de7e38617b6c420a45a84c4") }))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(Messages.GroupNameMustBeUnique);
        }
        [Test]
        public void WhenParentIdDoesNotExist_ThenThrowException()
        {
            var groupCommand = CreateUpdateGroupCommand(queryEntityById: Mock.Of<IQueryEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == false));

            var @group = new Group { Name = "test", ParentId = new Identity(Guid.NewGuid()) };
            groupCommand.Executing(gc => gc.Execute(@group))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(Messages.EntityNotFound);
        }

        [Test]
        public void WhenParentHasTheSameId_ThenThrowException()
        {
            var groupCommand = CreateUpdateGroupCommand(queryEntityById: Mock.Of<IQueryEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == true));

            var @group = new Group
                             {
                                 Name = "test", 
                                 Id = new Identity("4de7e38617b6c420a45a84c4"), 
                                 ParentId = new Identity("4de7e38617b6c420a45a84c4")
                             };

            groupCommand.Executing(gc => gc.Execute(@group))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Messages.GroupCircleReference, group.Id));
        }


        [Test]
        public void WhenParentHasParentWithTheSameId_ThenThrowException()
        {
            var groupCommand = CreateUpdateGroupCommand(queryEntityById: Mock.Of<IQueryEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == true));

            var @group = new Group
            {
                Name = "test",
                Id = new Identity("4de7e38617b6c420a45a84c4"),
                ParentId = new Identity("4de7e38617b6c420a45a84c4")
            };

            groupCommand.Executing(gc => gc.Execute(@group))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Messages.GroupCircleReference, group.Id));
        }

        [Test]
        public void WhenEverythingIsOK_ThenUpdateTheGroup()
        {
            var stubCudOperations = new StubCudOperations<Group>();
            var groupCommand = CreateUpdateGroupCommand(cudGroup: stubCudOperations);
            var @group = new Group { Name = "test" };
            groupCommand.Execute(@group);

            stubCudOperations.Updates.Should().Contain(@group);

        }

        private static IUpdateGroupCommand CreateUpdateGroupCommand(
            IExistGroupByGroupName existGroupByGroupName = null,
            IQueryEntityById queryEntityById = null,
            ICudOperations<Group> cudGroup = null)
        {
            return new UpdateGroupCommand(existGroupByGroupName ?? Mock.Of<IExistGroupByGroupName>(),
                                        queryEntityById ?? Mock.Of<IQueryEntityById>(),
                                        cudGroup ?? Mock.Of<ICudOperations<Group>>());
        }
    }
}