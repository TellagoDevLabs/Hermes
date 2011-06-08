using System;
using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Topics;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

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
            var stubRepository = new StubRepository<Topic>();
            var command = CreateCreateTopicCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Group>(groupId)),
                                    cudTopic: stubRepository);
            var topic = new Topic { Name = name, GroupId = groupId};
            command.Execute(topic);

            stubRepository.Entities.Should().Contain(topic);

        }

        private static ICreateTopicCommand CreateCreateTopicCommand(
            IExistsTopicByName existsTopicByName = null, 
            IEntityById entityById = null,
            IRepository<Topic> cudTopic = null)
        {
            return new CreateTopicCommand(existsTopicByName ?? Mock.Of<IExistsTopicByName>(),
                                        entityById ?? Mock.Of<IEntityById>(),
                                        cudTopic ?? Mock.Of<IRepository<Topic>>());
        }
    }
}