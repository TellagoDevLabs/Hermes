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