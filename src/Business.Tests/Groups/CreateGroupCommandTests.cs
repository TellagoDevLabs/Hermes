using System;
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
            Assert.Throws<ValidationException>(() => groupCommand.Create(new Group {Name = null}));
        }

        [Test]
        public void WhenGroupNameIsDuplicated_ThenThrowValidateException()
        {
            var groupCommand = CreateCreateGroupCommand(Mock.Of<IExistGroupByGroupName>(q => q.Execute("test") == true));

            groupCommand.Executing(gc => gc.Create(new Group {Name = "test"}))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(Messages.GroupNameMustBeUnique);
        }

        private ICreateGroupCommand CreateCreateGroupCommand(IExistGroupByGroupName existGroupByGroupName = null)
        {
            return new CreateGroupCommand(existGroupByGroupName ?? Mock.Of<IExistGroupByGroupName>());
        }
    }
}