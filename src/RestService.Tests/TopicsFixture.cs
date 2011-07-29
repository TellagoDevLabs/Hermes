using Autofac;
using Moq;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Topics;
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
    public class TopicsFixture : ResourceBaseFixture
    {
        private Mock<ICreateTopicCommand> mockedCreateCommand;
        private Mock<IGenericJsonPagedQuery> mockedGenericJsonQuery;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedCreateCommand = new Mock<ICreateTopicCommand>();
            mockedGenericJsonQuery = new Mock<IGenericJsonPagedQuery>();

            builder.RegisterInstance(new TopicsResource(
                mockedGenericJsonQuery.Object,
                mockedCreateCommand.Object));
        }

        protected override RestClient.SerializationType GetSerializationType()
        {
            return RestClient.SerializationType.Xml;
        }

        protected override Type GetServiceType()
        {
            return typeof(TopicsResource);
        }

        [Test]
        public void Should_get_all_topic()
        {
            var group = new M.Group { Id = Identity.Random() };
            var topics = new M.Topic[]
                             {
                                new M.Topic{
                                Description = "description 1",
                                GroupId = group.Id.Value ,
                                Id = Identity.Random(),
                                Name = "test 1"
                                },
                                new M.Topic{
                                Description = "description 2",
                                GroupId = group.Id.Value ,
                                Id = Identity.Random(),
                                Name = "test 2"
                                }
                             };

            mockedGenericJsonQuery.Setup(r => r.Execute<Topic>(null, null, null)).Returns(topics);

            var result = client.ExecuteGet<F.Topic[]>("");

            Assert.IsNotNull(topics);
            Assert.AreEqual(topics.Length, result.Length);
            Assert.IsTrue(topics.All(t1 => result.Any(t2 => t1.Id == t2.Id.ToModel() && t1.Name == t2.Name)));
        }

        [Test]
        public void Should_get_all_topic_filtered()
        {
            string query = "foo query";
            int? skip = null;
            int? limited = null;

            mockedGenericJsonQuery.Setup(r => r.Execute<Topic>(query, skip, limited)).Returns(new M.Topic[0]);

            var result = client.ExecuteGet<F.Topic[]>("?query=" + query);

            mockedGenericJsonQuery.Verify(r => r.Execute<Topic>(query, skip, limited));
        }

        [Test]
        public void Should_get_all_topic_paged()
        {
            string query = null;
            int? skip = 2;
            int? limited = 10;
            mockedGenericJsonQuery.Setup(r => r.Execute<Topic>(query, skip, limited)).Returns(new M.Topic[0]);

            var result = client.ExecuteGet<F.Topic[]>("?skip=" + skip.ToString() + "&limit=" + limited.ToString());

            mockedGenericJsonQuery.Verify(r => r.Execute<Topic>(query, skip, limited));
        }
        
        [Test]
        public void Should_get_all_topic_filtered_and_paged()
        {
            string query = "foo query";
            int? skip = 2;
            int? limited = 10;
            mockedGenericJsonQuery.Setup(r => r.Execute<Topic>(query, skip, limited)).Returns(new M.Topic[0]);

            var result = client.ExecuteGet<F.Topic[]>("?query=" + query + "&skip=" + skip.ToString() + "&limit=" + limited.ToString());

            mockedGenericJsonQuery.Verify(r => r.Execute<Topic>(query, skip, limited));
        }

        [Test]
        public void Should_post_a_topic()
        {
           var topicPost = new F.TopicPost()
            {
                Description = "description",
                GroupId = F.Identity.Random(),
                Name = "test"
            };

           mockedCreateCommand.Setup(r => r.Execute(It.IsAny<M.Topic>())).Callback<M.Topic>(t => t.Id = Identity.Random());

            client.ExecutePost("", topicPost);

            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Topic>(t => t != null)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Topic>(t => t.Description == topicPost.Description)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Topic>(t => t.Name == topicPost.Name)));
            mockedCreateCommand.Verify(r => r.Execute(It.Is<M.Topic>(t => t.GroupId == topicPost.GroupId.ToModel())));
        }
    }
}
