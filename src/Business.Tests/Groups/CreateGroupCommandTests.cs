using System;
using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

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
                                    .Exception.Message.Should().Be.EqualTo(Texts.GroupNameMustBeUnique);
        }
        [Test]
        public void WhenParentIdDoesNotExist_ThenThrowException()
        {
            var groupCommand = CreateCreateGroupCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == false));

            var @group = new Group { Name = "test", ParentId  = new Identity(Guid.NewGuid())};
            groupCommand.Executing(gc => gc.Execute(@group))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(Texts.EntityNotFound);
        }

        [Test]
        public void WhenEverythingIsOK_ThenInsertTheGroup()
        {
            var stubRepository = new StubRepository<Group>();
            var groupCommand = CreateCreateGroupCommand(cudGroup: stubRepository);
            var @group = new Group { Name = "test"};
            groupCommand.Execute(@group);

            stubRepository.Entities.Should().Contain(@group);

        }

        private static ICreateGroupCommand CreateCreateGroupCommand(
            IExistGroupByGroupName existGroupByGroupName = null, 
            IEntityById entityById = null,
            IRepository<Group> cudGroup = null)
        {
            return new CreateGroupCommand(existGroupByGroupName ?? Mock.Of<IExistGroupByGroupName>(),
                                        entityById ?? Mock.Of<IEntityById>(),
                                        cudGroup ?? Mock.Of<IRepository<Group>>());
        }
    }
}