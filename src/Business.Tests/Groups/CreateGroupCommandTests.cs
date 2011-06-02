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
    public class CreateGroupCommandTests
    {
        [Test]
        public void WhenNameIsNull_ThenThrowValidateException()
        {
            var groupCommand = CreateCreateGroupCommand();
            Assert.Throws<ValidationException>(() => groupCommand.Execute(new Group {Name = null}));
        }

        [Test]
        public void WhenGroupNameIsDuplicated_ThenThrowValidateException()
        {
            var groupCommand = CreateCreateGroupCommand(Mock.Of<IExistGroupByGroupName>(q => q.Execute("test", null) == true));

            groupCommand.Executing(gc => gc.Execute(new Group {Name = "test"}))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(Messages.GroupNameMustBeUnique);
        }
        [Test]
        public void WhenParentIdDoesNotExist_ThenThrowException()
        {
            var groupCommand = CreateCreateGroupCommand(queryEntityById: Mock.Of<IQueryEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == false));

            var @group = new Group { Name = "test", ParentId  = new Identity(Guid.NewGuid())};
            groupCommand.Executing(gc => gc.Execute(@group))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(Messages.EntityNotFound);
        }

        [Test]
        public void WhenEverythingIsOK_ThenInsertTheGroup()
        {
            var stubCudOperations = new StubCudOperations<Group>();
            var groupCommand = CreateCreateGroupCommand(cudGroup: stubCudOperations);
            var @group = new Group { Name = "test"};
            groupCommand.Execute(@group);

            stubCudOperations.Entities.Should().Contain(@group);

        }

        private static ICreateGroupCommand CreateCreateGroupCommand(
            IExistGroupByGroupName existGroupByGroupName = null, 
            IQueryEntityById queryEntityById = null,
            ICudOperations<Group> cudGroup = null)
        {
            return new CreateGroupCommand(existGroupByGroupName ?? Mock.Of<IExistGroupByGroupName>(),
                                        queryEntityById ?? Mock.Of<IQueryEntityById>(),
                                        cudGroup ?? Mock.Of<ICudOperations<Group>>());
        }
    }
}