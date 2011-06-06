using System;
using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Topics;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.Business.Topics.Queries;

namespace Business.Tests.Topics
{
    [TestFixture]
    public class CreateTopicCommandTests
    {
        [Test]
        public void WhenNameIsNull_ThenThrowValidateException()
        {
            var command = CreateCreateTopicCommand();
            Assert.Throws<ValidationException>(() => command.Execute(new Topic {Name = null}));
        }

        [Test]
        public void WhenTopicNameIsDuplicated_ThenThrowValidateException()
        {
            var name = "test";
            var command = CreateCreateTopicCommand(Mock.Of<IExistsTopicByName>(q => q.Execute(name, null) == true));

            command.Executing(gc => gc.Execute(new Topic {Name = name}))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Messages.TopicNameMustBeUnique, name));
        }
        [Test]
        public void WhengGroupIdDoesNotExist_ThenThrowException()
        {
            var name = "Test";
            var groupId = Identity.Random();
            var command = CreateCreateTopicCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Topic>(groupId) == false));

            var topic = new Topic { Name = name, GroupId  = groupId};
            command.Executing(gc => gc.Execute(topic))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Messages.EntityNotFound, typeof(Group).Name, groupId));
        }

        [Test]
        public void WhenEverythingIsOK_ThenInsertTheTopic()
        {
            var name = "Test";
            var groupId = Identity.Random();
            var stubCudOperations = new StubCudOperations<Topic>();
            var command = CreateCreateTopicCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Group>(groupId)),
                                    cudTopic: stubCudOperations);
            var topic = new Topic { Name = name, GroupId = groupId};
            command.Execute(topic);

            stubCudOperations.Documents.Should().Contain(topic);

        }

        private static ICreateTopicCommand CreateCreateTopicCommand(
            IExistsTopicByName existsTopicByName = null, 
            IEntityById entityById = null,
            ICudOperations<Topic> cudTopic = null)
        {
            return new CreateTopicCommand(existsTopicByName ?? Mock.Of<IExistsTopicByName>(),
                                        entityById ?? Mock.Of<IEntityById>(),
                                        cudTopic ?? Mock.Of<ICudOperations<Topic>>());
        }
    }
}