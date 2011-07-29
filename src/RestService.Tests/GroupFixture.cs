using Autofac;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.RestService.Extensions;
using TellagoStudios.Hermes.RestService.Resources;
using M = TellagoStudios.Hermes.Business.Model;
using F = TellagoStudios.Hermes.Facade;
using System;
using System.Linq;
using System.Net;

namespace RestService.Tests
{
    [TestFixture]
    public class GroupFixture : ResourceBaseFixture
    {
        private Mock<IUpdateGroupCommand> mockedUpdateCommand;
        private Mock<IDeleteGroupCommand> mockedDeleteCommand;
        private Mock<IEntityById> mockedEntityById;
        private Mock<IGroupByName> mockedGroupByName;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedUpdateCommand = new Mock<IUpdateGroupCommand>();
            mockedDeleteCommand = new Mock<IDeleteGroupCommand>();
            mockedEntityById = new Mock<IEntityById>();
            mockedGroupByName = new Mock<IGroupByName>();
            builder.RegisterInstance(new GroupResource(
                mockedEntityById.Object,
                mockedUpdateCommand.Object,
                mockedDeleteCommand.Object, 
                Mock.Of<ITopicsByGroup>(),
                mockedGroupByName.Object));
        }

        protected override RestClient.SerializationType GetSerializationType()
        {
            return RestClient.SerializationType.Xml;
        }

        protected override Type GetServiceType()
        {
            return typeof(GroupResource);
        }

        [Test]
        public void Should_get_a_group_by_id()
        {
            var parent = new M.Group { Id = M.Identity.Random() };
            var group = new M.Group
                            {
                                Id = M.Identity.Random(),
                                Name = "sample",
                                Description = "sample group",
                                ParentId = parent.Id
                            };

            mockedEntityById.Setup(r => r.Get<Group>(group.Id.Value)).Returns(group);

            var result = client.ExecuteGet<F.Group>("/" + group.Id);

            Assert.AreEqual(group.Description, result.Description);
            Assert.AreEqual(group.Id, result.Id.ToModel());
            Assert.AreEqual(group.Name, result.Name);
            Assert.IsNotEmpty(result.Links);

            result.Links.Satisfy(ls => ls.Any(l =>
                                              l.Relation == TellagoStudios.Hermes.RestService.Constants.Relationships.Parent
                                              && l.Uri == ResourceLocation.OfGroup(parent.Id.Value).ToString()));
        }

        [Test]
        public void Validates_a_get_with_an_invalid_id()
        {
            var id = M.Identity.Random();
            mockedEntityById.Setup(r => r.Get<Group>(It.IsAny<M.Identity>())).Returns((M.Group)null);

            var result = client.ExecuteGet<F.Group>("/" + id, HttpStatusCode.NotFound);

            Assert.IsNull(result);
        }

        [Test]
        public void Should_put_a_topic()
        {
            var groupPut = new F.GroupPut()
                               {
                                   Id = F.Identity.Random(),
                                   Description = "description",
                                   ParentId = F.Identity.Random(),
                                   Name = "test"
                               };

            var parent = new M.Group {Id = groupPut.ParentId.ToModel()};
            var group = new M.Group()
                            {
                                Description = groupPut.Description,
                                ParentId = parent.Id,
                                Id = groupPut.Id.ToModel(),
                                Name = groupPut.Name
                            };


            mockedUpdateCommand.Setup(r => r.Execute(It.IsAny<M.Group>())); //.Returns(group);

            client.ExecutePut<F.GroupPut>(groupPut.Id.ToString(), groupPut);

            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t != null)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Description == groupPut.Description)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Name == groupPut.Name)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.ParentId == groupPut.ParentId.ToModel())));
        }

        [Test]
        public void Validates_a_put_with_a_ValidationError()
        {
            var groupPut = new F.GroupPut()
            {
                Id = F.Identity.Random(),
                Description = "description",
                Name = string.Empty,
                ParentId = F.Identity.Random()
            };

            mockedUpdateCommand.Setup(r => r.Execute(It.IsAny<M.Group>())).Throws(new ValidationException("foo"));

            var result = client.ExecutePut<F.GroupPut, F.Group>(groupPut.Id.ToString(), groupPut, HttpStatusCode.BadRequest);

            Assert.IsNull(result);

            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t != null)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Id == groupPut.Id.ToModel())));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Description == groupPut.Description)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Name == groupPut.Name)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.ParentId == groupPut.ParentId.ToModel())));
        }

        [Test]
        public void Validates_a_put_with_an_invalid_GroupId()
        {
            var groupPut = new F.GroupPut()
            {
                Id = F.Identity.Random(),
                Description = "description",
                Name = string.Empty,
                ParentId = F.Identity.Random()
            };

            mockedUpdateCommand.Setup(r => r.Execute(It.IsAny<M.Group>())).Throws<EntityNotFoundException>();

            var result = client.ExecutePut<F.GroupPut, F.Group>(groupPut.Id.ToString(), groupPut, HttpStatusCode.NotFound);

            Assert.IsNull(result);

            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t != null)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Id == groupPut.Id.ToModel())));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Description == groupPut.Description)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Name == groupPut.Name)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.ParentId == groupPut.ParentId.ToModel())));
        }
        
        [Test]
        public void Should_delete_a_group()
        {
            var id = M.Identity.Random(12);
            client.ExecuteDelete("/" + id);

            mockedDeleteCommand.Verify(r => r.Execute(id));
        }

        [Test]
        public void Should_get_a_topic_by_name_and_without_group()
        {
            var group = new M.Group()
            {
                Description = "description",
                Id = Identity.Random(),
                Name = "test"
            };
            mockedGroupByName.Setup(r => r.Get(group.Name)).Returns(group);

            var result = client.ExecuteGet<F.Group>("/?name=" + group.Name);

            Assert.AreEqual(group.Description, result.Description);
            Assert.AreEqual(group.Id, result.Id.ToModel());
            Assert.AreEqual(group.Name, result.Name);
        }

    }
}
