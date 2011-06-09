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
        private Mock<IUpdateTopicCommand> mockedUpdateCommand;
        private Mock<IDeleteTopicCommand> mockedDeleteCommand;
        private Mock<IEntityById> mockedEntityById;
        private Mock<IGenericJsonPagedQuery> mockedGenericJsonQuery;
        private Mock<ITopicsByGroup> mockedTopicsByGroup;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedCreateCommand = new Mock<ICreateTopicCommand>();
            mockedUpdateCommand = new Mock<IUpdateTopicCommand>();
            mockedDeleteCommand = new Mock<IDeleteTopicCommand>();
            mockedEntityById = new Mock<IEntityById>();
            mockedTopicsByGroup = new Mock<ITopicsByGroup>();
            mockedGenericJsonQuery = new Mock<IGenericJsonPagedQuery>();
            builder.RegisterInstance(new TopicsResource(
                mockedEntityById.Object,
                mockedGenericJsonQuery.Object,
                mockedCreateCommand.Object,
                mockedUpdateCommand.Object,
                mockedDeleteCommand.Object,
                mockedTopicsByGroup.Object));
        }

        protected override Type GetServiceType()
        {
            return typeof(TopicsResource);
        }

        [Test]
        public void Should_get_a_topic_by_id()
        {
            var group = new M.Group { Id = Identity.Random() };
            var topic = new M.Topic()
                            {
                                Description = "description",
                                GroupId = group.Id.Value ,
                                Id = Identity.Random(),
                                Name = "test"
                            };
            mockedEntityById.Setup(r => r.Get<Topic>(topic.Id.Value)).Returns(topic);

            var result = client.ExecuteGet<F.Topic>("/" + topic.Id);

            Assert.AreEqual(topic.Description, result.Description);
            Assert.AreEqual(TellagoStudios.Hermes.RestService.Constants.Relationships.Group, result.Group.Rel);
            Assert.AreEqual(ResourceLocation.OfGroup(topic.GroupId), result.Group.HRef);
            Assert.AreEqual(topic.Id, result.Id.ToModel());
            Assert.AreEqual(topic.Name, result.Name);
        }

       [Test]
       public void Validates_a_get_with_an_invalid_id()
        {
            var id = Identity.Random();
            mockedEntityById.Setup(r => r.Get<Topic>(It.IsAny<Identity>())).Throws<EntityNotFoundException>();

            var result = client.ExecuteGet<F.Topic>("/" + id, HttpStatusCode.NotFound);

            Assert.IsNull(result);
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

        [Test]
        public void Should_put_a_topic()
        {
            var topicPut = new F.TopicPut()
            {
                Id = F.Identity.Random(),
                Description = "description",
                GroupId = F.Identity.Random(),
                Name = "test"
            };

            var group = new M.Group { Id = M.Identity.Random() };
            var topic = new M.Topic()
            {
                Description = topicPut.Description,
                GroupId = group.Id.Value,
                Id = topicPut.Id.ToModel(),
                Name = topicPut.Name
            };


            mockedUpdateCommand.Setup(r => r.Execute(It.IsAny<M.Topic>()));

            client.ExecutePut("", topicPut);

            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Topic>(t => t != null)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Topic>(t => t.Description == topicPut.Description)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Topic>(t => t.Name == topicPut.Name)));
            mockedUpdateCommand.Verify(r => r.Execute(It.Is<M.Topic>(t => t.GroupId == topicPut.GroupId.ToModel())));
        }

        [Test]
        public void Should_delete_a_topic()
        {
            var topicId = Identity.Random();

            client.ExecuteDelete("/" + topicId);

            mockedDeleteCommand.Verify(r => r.Execute(topicId));
        }
    }
}
