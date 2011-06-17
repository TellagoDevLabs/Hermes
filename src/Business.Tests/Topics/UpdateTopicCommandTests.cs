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
    public class UpdateTopicCommandTests
    {
        [Test]
        public void WhenIdIsNull_ThenThrowValidateException()
        {
            var command = CreateUpdateTopicCommand();
            var topic = new Topic { Id = null };
            command.Executing(c => c.Execute(topic))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Texts.IdMustNotBeNull));
        }

        [Test]
        public void WhenIdIsInvalid_ThenThrowEntityNotFoundException()
        {
            var id = Identity.Random();
            var command = CreateUpdateTopicCommand(entityById: Mock.Of<IEntityById>( q => q.Exist<Topic>(id) == false));
            var topic = new Topic { Id = id };
            command.Executing(c => c.Execute(topic))
                                    .Throws<EntityNotFoundException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Texts.EntityNotFound, typeof(Topic).Name, id));
        }

        [Test]
        public void WhenNameIsNull_ThenThrowValidateException()
        {
            var id = Identity.Random();
             
            var command = CreateUpdateTopicCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Topic>(id)));

            var topic = new Topic { Id = id, Name = null };
            command.Executing(c => c.Execute(topic))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Texts.NameMustBeNotNull));
        }

        [Test]
        public void WhenTopicNameIsDuplicated_ThenThrowValidateException()
        {
            var id = Identity.Random();
            var name = "test";
            var command = CreateUpdateTopicCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Topic>(id)),
            existsTopicByName: Mock.Of<IExistsTopicByName>(q => q.Execute(name, id) ));

            command.Executing(c => c.Execute(new Topic {Id = id, Name = name}))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Texts.TopicNameMustBeUnique, name));
        }
        [Test]
        public void WhengGroupIdDoesNotExist_ThenThrowException()
        {
            var id = Identity.Random();
            var name = "Test";
            var groupId = Identity.Random();
            var command = CreateUpdateTopicCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Topic>(id)),
                existsTopicByName: Mock.Of<IExistsTopicByName>(q => q.Execute(name, id)== false));

            var topic = new Topic { Id = id, Name = name, GroupId  = groupId};
            command.Executing(c => c.Execute(topic))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Texts.EntityNotFound, typeof(Group).Name, groupId));
        }

        [Test]
        public void WhenEverythingIsOK_ThenInsertTheTopic()
        {
            var stubRepository = new StubRepository<Topic>();
            var id = Identity.Random();
            var name = "Test";
            var groupId = Identity.Random();
            var command = CreateUpdateTopicCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Topic>(id) && q.Exist<Group>(groupId)),
                existsTopicByName: Mock.Of<IExistsTopicByName>(q => q.Execute(name, id)==false), cudTopic: stubRepository);
            var topic = new Topic { Id = id, Name = name, GroupId = groupId};

            command.Execute(topic);

            stubRepository.Updates.Should().Contain(topic);
        }

        private static IUpdateTopicCommand CreateUpdateTopicCommand(
            IExistsTopicByName existsTopicByName = null, 
            IEntityById entityById = null,
            IRepository<Topic> cudTopic = null)
        {
            return new UpdateTopicCommand(existsTopicByName ?? Mock.Of<IExistsTopicByName>(),
                                        entityById ?? Mock.Of<IEntityById>(),
                                        cudTopic ?? Mock.Of<IRepository<Topic>>());
        }
    }
}