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
    [Ignore]
    public class JsonFixture : ResourceBaseFixture
    {
        private Mock<ICreateGroupCommand> mockedCreateCommand;
        private Mock<IUpdateGroupCommand> mockedUpdateCommand;
        private Mock<IDeleteGroupCommand> mockedDeleteCommand;
        private Mock<IEntityById> mockedEntityById;
        private Mock<IGenericJsonPagedQuery> mockedGenericJsonQuery;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedCreateCommand = new Mock<ICreateGroupCommand>();
            mockedUpdateCommand = new Mock<IUpdateGroupCommand>();
            mockedDeleteCommand = new Mock<IDeleteGroupCommand>();
            mockedEntityById = new Mock<IEntityById>();
            mockedGenericJsonQuery = new Mock<IGenericJsonPagedQuery>();
            builder.RegisterInstance(new GroupsResource(
                mockedEntityById.Object,
                mockedGenericJsonQuery.Object,
                mockedCreateCommand.Object,
                mockedUpdateCommand.Object,
                mockedDeleteCommand.Object, Mock.Of<ITopicsByGroup>()));
        }

        protected override RestClient.SerializationType GetSerializationType()
        {
            return RestClient.SerializationType.Json;
        }

        protected override Type GetServiceType()
        {
            return typeof(GroupsResource);
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
        public void Should_post_a_topic_group()
        {
            //TODO fix
            var groupPost = new F.GroupPost()
            {
                Description = "description",
                Name = "test",
                ParentId = F.Identity.Random()
            };

            var parent = new M.Group { Id = groupPost.ParentId.ToModel() };
            
            mockedCreateCommand.Setup(r => r.Execute(It.IsAny<M.Group>()))
                .Callback<M.Group>(g => g.Id = new Identity("4de7e38617b6c420a45a84c4"));

            var result = client.ExecutePost("", groupPost);

            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(g => g.Satisfy(gr => gr != null 
                                                                                    && gr.Description == groupPost.Description
                                                                                    && gr.Name == groupPost.Name
                                                                                    && gr.ParentId == groupPost.ParentId.ToModel()))));

            Assert.IsNotNull(result);
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


            mockedUpdateCommand.Setup(r => r.Execute(It.IsAny<M.Group>()));
            client.ExecutePut(groupPut.Id.ToString(), groupPut);

            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t != null)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Description == groupPut.Description)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Name == groupPut.Name)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.ParentId == groupPut.ParentId.ToModel())));
        }
    }
}
