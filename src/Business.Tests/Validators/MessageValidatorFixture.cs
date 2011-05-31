using System;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.Business.Validator;
using TellagoStudios.Hermes.Business.Exceptions;
using Moq;
using NUnit.Framework;

namespace Business.Tests.Validators
{
    [TestFixture]
    public class MessageValidatorFixture
    {
        private Mock<ITopicService> topicService;
        private Mock<IGroupService> groupService;
        private Mock<ISubscriptionService> subscriptionService;
        private MessageValidator validator;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            topicService = new Mock<ITopicService>(MockBehavior.Loose);
            groupService = new Mock<IGroupService>(MockBehavior.Loose);
            subscriptionService = new Mock<ISubscriptionService>(MockBehavior.Loose);

            validator = new MessageValidator
            {
                TopicService = topicService.Object,
                GroupService = groupService.Object,
                SubscriptionService = subscriptionService.Object
            };
        }

        [Test]
        public void ValidateSubsriptionExists_should_pass_on_valid_id()
        {
            subscriptionService.Setup(ss => ss.ExistsById(It.IsAny<Identity>())).Returns(true);

            validator.ValidateSubsriptionExists(Identity.Random());
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateSubsriptionExists_should_fail_on_invalid_id()
        {
            subscriptionService.Setup(ss => ss.ExistsById(It.IsAny<Identity>())).Returns(false);

            validator.ValidateSubsriptionExists(Identity.Random());
        }

        [Test]
        public void ValidateBeforeCreate_should_pass_on_valid_message()
        {
            var message = new Message
                              {
                                  UtcReceivedOn = DateTime.UtcNow,
                                  TopicId =Identity.Random()
                              };

            topicService.Setup(ts => ts.Exists(message.TopicId)).Returns(true);
            validator.ValidateBeforeCreate(message);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateBeforeCreate_should_fail_on_empty_receivedOn_field()
        {
            var message = new Message
            {
                TopicId = Identity.Random()
            };

            topicService.Setup(ts => ts.Exists(message.TopicId)).Returns(true);
            validator.ValidateBeforeCreate(message);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateBeforeCreate_should_fail_on_empty_RecievedOn_field()
        {
            var message = new Message
            {
                TopicId = Identity.Random()
            };

            topicService.Setup(ts => ts.Exists(message.TopicId)).Returns(true);
            validator.ValidateBeforeCreate(message);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateBeforeCreate_should_fail_on_empty_TopicId_field()
        {
            var message = new Message
            {
                UtcReceivedOn = DateTime.UtcNow
            };

            validator.ValidateBeforeCreate(message);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateBeforeCreate_should_fail_on_invalid_TopicID_field()
        {
            var message = new Message
            {
                UtcReceivedOn = DateTime.UtcNow,
                TopicId = Identity.Random()
            };

            topicService.Setup(ts => ts.Exists(message.TopicId)).Returns(false);
            validator.ValidateBeforeCreate(message);
        }

        [Test]
        public void ValidateBeforeGet_should_pass_on_valid_key()
        {
            var key = new MessageKey
            {
                MessageId = Identity.Random(),
                TopicId = Identity.Random()
            };

            topicService.Setup(ts => ts.Exists(key.TopicId)).Returns(true);
            validator.ValidateBeforeGet(key);
        }

        [ExpectedException(typeof(ValidationException))]
        public void ValidateBeforeGet_should_pass_on_invalid_topicId()
        {
            var key = new MessageKey
            {
                MessageId = Identity.Random(),
                TopicId = Identity.Random()
            };

            topicService.Setup(ts => ts.Exists(key.TopicId)).Returns(false);
            validator.ValidateBeforeGet(key);
        }

        [Test]
        public void ValidateGroup_should_pass_on_valid_id()
        {
            groupService.Setup(tgs => tgs.Exists(It.IsAny<Identity>())).Returns(true);

            validator.ValidateGroup(Identity.Random());
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateGroup_should_fail_on_invalid_id()
        {
            groupService.Setup(tgs => tgs.Exists(It.IsAny<Identity>())).Returns(false);

            validator.ValidateGroup(Identity.Random());
        }
    }
}
