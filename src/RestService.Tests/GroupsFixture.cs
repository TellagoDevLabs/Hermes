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
        private Mock<IGenericJsonPagedQuery> mockedGenericJsonQuery;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedCreateCommand = new Mock<ICreateGroupCommand>();
            mockedGenericJsonQuery = new Mock<IGenericJsonPagedQuery>();
            builder.RegisterInstance(new GroupsResource(
                mockedGenericJsonQuery.Object,
                mockedCreateCommand.Object));
        }

        protected override RestClient.SerializationType GetSerializationType()
        {
            return RestClient.SerializationType.Xml;
        }

        protected override Type GetServiceType()
        {
            return typeof(GroupsResource);
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

            var result = client.ExecutePost("", groupPost);

            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(g => g.Satisfy(gr => gr != null 
                                                                                    && gr.Description == groupPost.Description
                                                                                    && gr.Name == groupPost.Name
                                                                                    && gr.ParentId == groupPost.ParentId.ToModel()))));
            
            Assert.IsNotNull(result);
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

            var result = client.ExecutePost("", groupPost, HttpStatusCode.BadRequest);

            Assert.IsNull(result);

            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t != null)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Description == groupPost.Description)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.Name == groupPost.Name)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Group>(t => t.ParentId == groupPost.ParentId.ToModel())));
        }
    }
}
