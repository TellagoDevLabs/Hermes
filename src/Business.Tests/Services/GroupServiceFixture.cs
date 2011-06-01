using System;
using System.Linq;
using TellagoStudios.Hermes.Business.Exceptions;
using Moq;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Validator;

namespace Business.Tests.Services
{
    [TestFixture]
    public class GroupServiceFixture
    {
        private GroupService service;
        private Mock<IGroupRepository> mockedRepository;
        private Mock<ITopicService> mockedTopicService;

        [SetUp]
        public void SetUpFixture()
        {
            mockedTopicService = new Mock<ITopicService>(MockBehavior.Loose);
            mockedRepository = new Mock<IGroupRepository>(MockBehavior.Loose);

            service = new GroupService
            {
                Repository = mockedRepository.Object,
                Validator = new GroupValidator
                {
                    Repository = mockedRepository.Object,
                    TopicService = mockedTopicService.Object
                }
            };
        }
        
        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void Should_throw_an_exception_when_deleting_a_group_with_topics()
        {
            var groupId = Identity.Random();

            mockedRepository.Setup(r => r.ExistsById(groupId)).Returns(true);
            mockedTopicService.Setup(ts => ts.ExistsByGroup(groupId)).Returns(true);

            service.Delete(groupId);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void Should_throw_an_exception_when_deleting_a_group_with_subgroups()
        {
            var groupId = Identity.Random();

            mockedRepository.Setup(r => r.ExistsById(groupId)).Returns(true);
            mockedTopicService.Setup(ts => ts.ExistsByGroup(groupId)).Returns(false);
            mockedRepository.Setup(r => r.ExistsByQuery(It.IsAny<string>())).Returns(true);

            service.Delete(groupId);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void Should_throw_an_exception_when_updating_a_group_with_a_loop()
        {
            var group1 = new Group
            {
                Id = Identity.Random(),
                Name = "parent name",
                Description = "parent description"
            };

            var group2 = new Group
            {
                Id = Identity.Random(),
                Name = "name",
                Description = "description",
                ParentId = group1.Id
            };

            var group3 = new Group
            {
                Id = Identity.Random(),
                Name = "name",
                Description = "description",
                ParentId = group2.Id
            };

            group1.ParentId = group3.Id;

            mockedRepository.Setup(r => r.ExistsById(group1.Id.Value)).Returns(true);
            mockedRepository.Setup(r => r.Get(group1.Id.Value)).Returns(group1);
            mockedRepository.Setup(r => r.Get(group2.Id.Value)).Returns(group2);
            mockedRepository.Setup(r => r.Get(group3.Id.Value)).Returns(group3);

            var result = service.Update(group1);
        }

        [Test]
        public void Should_delete_a_group()
        {
            var groupId = Identity.Random();

            mockedRepository.Setup(r => r.ExistsById(groupId)).Returns(true);
            mockedTopicService.Setup(ts => ts.ExistsByGroup(groupId)).Returns(false);
            mockedRepository.Setup(r => r.ExistsByQuery(It.Is<string>(s => !string.IsNullOrWhiteSpace(s)))).Returns(false);

            service.Delete(groupId);

            mockedRepository.Verify(r => r.Delete(groupId));
        }

        [Test]
        public void Should_update_a_group()
        {
            var parent = new Group
                             {
                                 Id = Identity.Random(),
                                 Name = "parent name",
                                 Description = "parent description"
                             };
            var group = new Group
                            {
                                Id = Identity.Random(),
                                Name = "name",
                                Description = "description",
                                ParentId = parent.Id
                            };

            mockedRepository.Setup(r => r.ExistsById(group.Id.Value)).Returns(true);
            mockedRepository.Setup(r => r.ExistsByQuery(It.Is<string>(s=>!string.IsNullOrWhiteSpace(s)))).Returns(false);
            mockedRepository.Setup(r => r.Get(parent.Id.Value)).Returns(parent);
            mockedRepository.Setup(r => r.Update(group)).Returns(group);
            
            var result = service.Update(group);
            
            mockedRepository.Verify(r => r.Update(group));
        }

        [Test]
        public void Can_return_all_Groups_for_a_Topic()
        {
            var parent = new Group
                             {
                                 Id = Identity.Random(),
                                 Name = "parent",
                                 Description = "parent"
                             };
            var grandParent = new Group
                                  {
                                      Id = Identity.Random(),
                                      Name = "grand parent",
                                      Description = "grand parent"
                                  };
            var greatGrandParent = new Group
                                       {
                                           Id = Identity.Random(),
                                           Name = "great grand parent",
                                           Description = "great grand parent"
                                       };

            parent.ParentId = grandParent.Id;
            grandParent.ParentId = greatGrandParent.Id;

            mockedRepository.Setup(r => r.Get(parent.Id.Value)).Returns(parent);
            mockedRepository.Setup(r => r.Get(grandParent.Id.Value)).Returns(grandParent);
            mockedRepository.Setup(r => r.Get(greatGrandParent.Id.Value)).Returns(greatGrandParent);

            var groups = service.GetAncestry(parent.Id.Value).ToList();

            Assert.IsNotEmpty(groups);
        }

    }
}
