using Autofac;
using Moq;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.RestService;
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
        private Mock<IGroupService> mockedService;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedService = new Mock<IGroupService>(MockBehavior.Loose);
            builder.RegisterInstance(new GroupsResource(mockedService.Object));
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

            mockedService.Setup(r => r.Get(group.Id.Value)).Returns(group);

            var result = client.ExecuteGet<F.Group>("/" + group.Id);

            mockedService.Verify(r => r.Get(group.Id.Value));

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
            mockedService.Setup(r => r.Get(It.IsAny<M.Identity>())).Returns((M.Group)null);

            var result = client.ExecuteGet<F.Group>("/" + id, HttpStatusCode.NotFound);

            mockedService.Verify(r => r.Get(id));

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
            mockedService.Setup(r => r.Find(null, null, null)).Returns(groups);

            var result = client.ExecuteGet<F.Group[]>("");

            mockedService.Verify(r => r.Find(null, null, null));

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

            mockedService.Setup(r => r.Find(query, skip, limited)).Returns(new M.Group[0]);

            var result = client.ExecuteGet<F.Group[]>("?query=" + query);

            mockedService.Verify(r => r.Find(query, skip, limited));
        }

        [Test]
        public void Should_get_all_topic_groups_paged()
        {
            string query = null;
            int? skip = 2;
            int? limited = 10;
            mockedService.Setup(r => r.Find(query, skip, limited)).Returns(new M.Group[0]);

            var result = client.ExecuteGet<F.Group[]>("?skip=" + skip.ToString() + "&limit=" + limited.ToString());

            mockedService.Verify(r => r.Find(query, skip, limited));
        }

        [Test]
        public void Should_get_all_topic_groups_filtered_and_paged()
        {
            string query = "foo query";
            int? skip = 2;
            int? limited = 10;
            mockedService.Setup(r => r.Find(query, skip, limited)).Returns(new M.Group[0]);

            var result = client.ExecuteGet<F.Group[]>("?query=" + query + "&skip=" + skip.ToString() + "&limit=" + limited.ToString());

            mockedService.Verify(r => r.Find(query, skip, limited));
        }

        [Test]
        public void Should_post_a_topic_group()
        {
            var groupPost = new F.GroupPost()
            {
                Description = "description",
                Name = "test",
                ParentId = F.Identity.Random()
            };

            var parent = new M.Group { Id = groupPost.ParentId.ToModel() };
            var group = new M.Group()
            {
                Description = groupPost.Description,
                Id = M.Identity.Random(),
                Name = groupPost.Name,
                ParentId = parent.Id

            };


            mockedService.Setup(r => r.Create(It.IsAny<M.Group>())).Returns(group);

            var result = client.ExecutePost<F.GroupPost, F.Group>("", groupPost);

            mockedService.Verify(r => r.Create(It.Is<M.Group>(t => t != null)));
            mockedService.Verify(r => r.Create(It.Is<M.Group>(t => t.Description == groupPost.Description)));
            mockedService.Verify(r => r.Create(It.Is<M.Group>(t => t.Name == groupPost.Name)));
            mockedService.Verify(r => r.Create(It.Is<M.Group>(t => t.ParentId == groupPost.ParentId.ToModel())));

            Assert.AreEqual(group.Description, result.Description);
            Assert.AreEqual(TellagoStudios.Hermes.RestService.Constants.Relationships.Parent, result.Parent.rel);
            Assert.AreEqual(ResourceLocation.OfGroup(parent.Id.Value), result.Parent.href);
            Assert.AreEqual(group.Id, result.Id.ToModel());
            Assert.AreEqual(group.Name, result.Name);
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

            mockedService.Setup(r => r.Create(It.IsAny<M.Group>())).Throws(new ValidationException("foo"));

            var result = client.ExecutePost<F.GroupPost, F.Group>("", groupPost, HttpStatusCode.BadRequest);

            Assert.IsNull(result);

            mockedService.Verify(r => r.Create(It.Is<M.Group>(t => t != null)));
            mockedService.Verify(r => r.Create(It.Is<M.Group>(t => t.Description == groupPost.Description)));
            mockedService.Verify(r => r.Create(It.Is<M.Group>(t => t.Name == groupPost.Name)));
            mockedService.Verify(r => r.Create(It.Is<M.Group>(t => t.ParentId == groupPost.ParentId.ToModel())));
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


            mockedService.Setup(r => r.Update(It.IsAny<M.Group>())).Returns(group);

            var result = client.ExecutePut<F.GroupPut, F.Group>("", groupPut);

            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t != null)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.Description == groupPut.Description)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.Name == groupPut.Name)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.ParentId == groupPut.ParentId.ToModel())));

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

            mockedService.Setup(r => r.Update(It.IsAny<M.Group>())).Throws(new ValidationException("foo"));

            var result = client.ExecutePut<F.GroupPut, F.Group>("", groupPut, HttpStatusCode.BadRequest);

            Assert.IsNull(result);

            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t != null)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.Id == groupPut.Id.ToModel())));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.Description == groupPut.Description)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.Name == groupPut.Name)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.ParentId == groupPut.ParentId.ToModel())));
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

            mockedService.Setup(r => r.Update(It.IsAny<M.Group>())).Throws<EntityNotFoundException>();

            var result = client.ExecutePut<F.GroupPut, F.Group>("", groupPut, HttpStatusCode.NotFound);

            Assert.IsNull(result);

            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t != null)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.Id == groupPut.Id.ToModel())));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.Description == groupPut.Description)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.Name == groupPut.Name)));
            mockedService.Verify(r => r.Update(It.Is<M.Group>(t => t.ParentId == groupPut.ParentId.ToModel())));
        }
        [Test]
        public void Should_delete_a_group()
        {
            var id = M.Identity.Random();

            mockedService.Setup(s => s.Exists(id)).Returns(true);

            client.ExecuteDelete("/" + id);

            mockedService.Verify(r => r.Delete(id));
        }
    }
}
