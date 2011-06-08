using System;
using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Messages;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace Business.Tests.Messages
{
    [TestFixture]
    public class CreateMessageCommandTests
    {
        [Test]
        public void WhenReveivedOnIsDefaultValue_ThenThrowValidateException()
        {
            var command = CreateCreateMessageCommand();
            command.Executing(cm => cm.Execute(new Message()))
                .Throws<ValidationException>()
                .And
                .Exception.Message.Should().Be.EqualTo(Texts.ReceivedOnMustBeSetted);
        }

        [Test]
        public void WhenTopicIdIsInvalid_ThenThrowValidateException()
        {
            var topicId = Identity.Random();
            var command = CreateCreateMessageCommand( Mock.Of<IEntityById>(q => q.Exist<Topic>(topicId) == false));

            var message = new Message 
            {
                UtcReceivedOn = DateTime.UtcNow, 
                TopicId = topicId
            };

            command.Executing(gc => gc.Execute(message))
                                    .Throws<EntityNotFoundException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Texts.EntityNotFound, typeof(Topic).Name, topicId));
        }
        [Test]
        public void WhenEverythingIsOK_ThenInsertTheMessage()
        {
            var stubRepository = new StubMessageRepository();
            var topicId = Identity.Random();
            var command = CreateCreateMessageCommand(Mock.Of<IEntityById>(q => q.Exist<Topic>(topicId) == true), stubRepository);

            var message = new Message
            {
                UtcReceivedOn = DateTime.UtcNow,
                TopicId = topicId
            };

            command.Execute(message);

            stubRepository.Documents.Should().Contain(message);

        }

        private static ICreateMessageCommand CreateCreateMessageCommand(
            IEntityById entityById = null,
            IMessageRepository cudMessage = null)
        {
            return new CreateMessageCommand(
                                        entityById ?? Mock.Of<IEntityById>(),
                                        cudMessage ?? Mock.Of<IMessageRepository>());
        }
    }
}