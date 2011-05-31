using System;
using System.Linq;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.Business.Validator;
using Moq;
using NUnit.Framework;

namespace Business.Tests.Validators
{
    [TestFixture]
    public class GroupValidatorTest
    {
        private Mock<IGroupRepository> _repository;
        private Mock<ITopicService> _topicService;
        private GroupValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IGroupRepository>(MockBehavior.Loose);
            _topicService = new Mock<ITopicService>(MockBehavior.Loose);
            _validator = new GroupValidator
            {
                Repository = _repository.Object,
                TopicService = _topicService.Object
            };
        }

        [Test]
        public void WillNotUpdateIfIdIsNotInDatabase()
        {
            var group = CreateWith(FakeId(Identity.Random()));
            Assert.Throws<EntityNotFoundException>(() => _validator.ValidateBeforeUpdate(group));
        }

        [Test]
        public void WillNotUpdateIfNameIsNull()
        {
            var group = CreateWith(ValidId(), Name(null));
            var exception = Assert.Throws<ValidationException>(() => _validator.ValidateBeforeUpdate(group));
            CollectionAssert.Contains(exception.Messages.ToList(), Messages.NameMustBeNotNull);
        }

        [Test]
        public void WillNotUpdateIfNameIsEmpty()
        {
            var group = CreateWith(ValidId(), Name(""));
            var exception = Assert.Throws<ValidationException>(() => _validator.ValidateBeforeUpdate(group));
            CollectionAssert.Contains(exception.Messages.ToList(), Messages.NameMustBeNotNull);
        }

        [Test]
        public void WillNotUpdateIfNameIsWhitespace()
        {
            var group = CreateWith(ValidId(), Name("    "));
            var exception = Assert.Throws<ValidationException>(() => _validator.ValidateBeforeUpdate(group));
            CollectionAssert.Contains(exception.Messages.ToList(), Messages.NameMustBeNotNull);
        }

        [Test]
        public void WillNotUpdateIfNameIsNotUnique()
        {
            var group = CreateWith(ValidId(), Name("Common Name"), NonUniqueName);
            var exception = Assert.Throws<ValidationException>(() => _validator.ValidateBeforeUpdate(group));
            CollectionAssert.Contains(exception.Messages.ToList(), string.Format(Messages.GroupNameMustBeUnique, group.Name));
        }

        [Test]
        public void WillNotUpdateIfThereIsACircularReference()
        {
            var group = CreateWith(ValidId(), Name("Common Name"), AncestryCycle);
            var exception = Assert.Throws<ValidationException>(() => _validator.ValidateBeforeUpdate(group));
            CollectionAssert.Contains(exception.Messages.ToList(), string.Format(Messages.GroupCircleReference, group.Id));
        }

        [Test]
        public void WillUpdateIfEverythingIsOk()
        {
            var group = CreateWith(ValidId(), Name("Just Fine"));
            Assert.DoesNotThrow(() => _validator.ValidateBeforeUpdate(group));
        }

        public Group CreateWith(params Action<Group>[] builders)
        {
            var group = new Group();
            builders.ForEach(builder => builder(group));
            return group;
        }

        private Action<Group> ValidId()
        {
            var id = Identity.Random();
            return group =>
            {
                group.Id = id;
                _repository.Setup(r => r.ExistsById(id)).Returns(true);
            };
        }

        private Action<Group> FakeId(Identity id)
        {
            return group =>
                       {
                           group.Id = id;
                           _repository.Setup(r => r.ExistsById(id)).Returns(false);
                       };
        }

        private static Action<Group> Name(string name)
        {
            return (group) => group.Name = name;
        }

        private void NonUniqueName(Group group)
        {
            // TODO replace by a query builder? mock it w/ an arg matcher?
            _repository.Setup(
                r =>
                r.ExistsByQuery(It.IsAny<string>()))
                .Returns(true);
        }

        private void AncestryCycle(Group group)
        {
            var parent = new Group();
            parent.Id = Identity.Random();
            parent.ParentId = group.Id;
            group.ParentId = parent.Id;

            _repository.Setup(r => r.Get(parent.Id.Value)).Returns(parent);
        }
    }
}