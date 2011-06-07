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
    public class GroupsFixture : ResourceBaseFixture
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
                mockedDeleteCommand.Object));
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
            Assert.IsNotNull(result.Parent);
            Assert.AreEqual(TellagoStudios.Hermes.RestService.Constants.Relationships.Parent, result.Parent.rel);
            Assert.AreEqual(ResourceLocation.OfGroup(parent.Id.Value), result.Parent.href);
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
        public void Should_get_all_topic_groups()
        {
            var parent = new M.Group { Id = M.Identity.Random() };
            var groups = new M.Group[]
                             {
                                new M.Group{
                                Description = "description 1",
                                Id = M.Identity.Random(),
                                Name = "test 1",
                                ParentId = parent.Id
                                },
                                new M.Group{
                                Description = "description 2",
                                Id = M.Identity.Random(),
                                Name = "test 2",
                                ParentId = parent.Id
                                }
                             };
            mockedGenericJsonQuery.Setup(r => r.Execute<Group>(null, null, null)).Returns(groups);

            var result = client.ExecuteGet<F.Group[]>("");

            mockedGenericJsonQuery.Verify(r => r.Execute<Group>(null, null, null));

            Assert.IsNotNull(groups);
            Assert.AreEqual(groups.Length, result.Length);
            Assert.IsTrue(groups.All(t1 => result.Any(t2 => t1.Id == t2.Id.ToModel() && t1.Name == t2.Name)));
        }

        [Test]
        public void Should_get_all_topic_groups_filtered()
        {
            string query = "foo query";
            int? skip = null;
            int? limited = null;

            mockedGenericJsonQuery.Setup(r => r.Execute<Group>(query, skip, limited)).Returns(new M.Group[0]);

            var result = client.ExecuteGet<F.Group[]>("?query=" + query);

            mockedGenericJsonQuery.Verify(r => r.Execute<Group>(query, skip, limited));
        }

        [Test]
        public void Should_get_all_topic_groups_paged()
        {
            string query = null;
            int? skip = 2;
            int? limited = 10;
            mockedGenericJsonQuery.Setup(r => r.Execute<Group>(query, skip, limited)).Returns(new M.Group[0]);

            var result = client.ExecuteGet<F.Group[]>("?skip=" + skip.ToString() + "&limit=" + limited.ToString());

            mockedGenericJsonQuery.Verify(r => r.Execute<Group>(query, skip, limited));
        }

        [Test]
        public void Should_get_all_topic_groups_filtered_and_paged()
        {
            string query = "foo query";
            int? skip = 2;
            int? limited = 10;
            mockedGenericJsonQuery.Setup(r => r.Execute<Group>(query, skip, limited)).Returns(new M.Group[0]);

            var result = client.ExecuteGet<F.Group[]>("?query=" + query + "&skip=" + skip.ToString() + "&limit=" + limited.ToString());

            mockedGenericJsonQuery.Verify(r => r.Execute<Group>(query, skip, limited));
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

            var result = client.ExecutePost<F.GroupPost, F.Group>("", groupPost);

            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(g => g.Satisfy(gr => gr != null 
                                                                                    && gr.Description == groupPost.Description
                                                                                    && gr.Name == groupPost.Name
                                                                                    && gr.ParentId == groupPost.ParentId.ToModel()))));
            
            //TODO: There are two things here: 1-Test asser many things. 2-Http speaking, a POST request should not be respondend with any content but a location in the header.
            //Assert.AreEqual(group.Description, result.Description);
            //Assert.AreEqual(TellagoStudios.Hermes.Business.Constants.Relationships.Parent, result.Parent.rel);
            //Assert.AreEqual(ResourceLocation.OfGroup(parent.Id.Value), result.Parent.href);
            //Assert.AreEqual(group.Id, result.Id.ToModel());
            //Assert.AreEqual(group.Name, result.Name);
        }

        [Test]
        public void Validates_a_post_with_a_ValidationError()
        {
            var groupPost = new F.GroupPost()
            {
                Description = "description",
                Name = string.Empty,
                ParentId = F.Identity.Random()
            };

            mockedCreateCommand.Setup(r => r.Execute(It.IsAny<M.Group>())).Throws(new ValidationException("foo"));

            var result = client.ExecutePost<F.GroupPost, F.Group>("", groupPost, HttpStatusCode.BadRequest);

            Assert.IsNull(result);

            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t != null)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Description == groupPost.Description)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Name == groupPost.Name)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.ParentId == groupPost.ParentId.ToModel())));
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

            var parent = new M.Group { Id = groupPut.ParentId.ToModel() };
            var group = new M.Group()
            {
                Description = groupPut.Description,
                ParentId = parent.Id,
                Id = groupPut.Id.ToModel(),
                Name = groupPut.Name
            };


            mockedUpdateCommand.Setup(r => r.Execute(It.IsAny<M.Group>()));//.Returns(group);

            var result = client.ExecutePut<F.GroupPut, F.Group>("", groupPut);

            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t != null)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Description == groupPut.Description)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Name == groupPut.Name)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.ParentId == groupPut.ParentId.ToModel())));

            Assert.AreEqual(group.Description, result.Description);
            Assert.AreEqual(TellagoStudios.Hermes.RestService.Constants.Relationships.Parent, result.Parent.rel);
            Assert.AreEqual(ResourceLocation.OfGroup(groupPut.ParentId.Value.ToModel()), result.Parent.href);
            Assert.AreEqual(group.Id, result.Id.ToModel());
            Assert.AreEqual(group.Name, result.Name);
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

            var result = client.ExecutePut<F.GroupPut, F.Group>("", groupPut, HttpStatusCode.BadRequest);

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

            var result = client.ExecutePut<F.GroupPut, F.Group>("", groupPut, HttpStatusCode.NotFound);

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
    }
}
