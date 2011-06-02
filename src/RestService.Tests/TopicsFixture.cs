using Autofac;
using Moq;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
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
    public class TopicsFixture : ResourceBaseFixture
    {
        private Mock<ITopicService> mockedService;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedService = new Mock<ITopicService>(MockBehavior.Loose);
            builder.RegisterInstance(new TopicsResource(mockedService.Object));
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
            mockedService.Setup(r => r.Get(topic.Id.Value)).Returns(topic);

            var result = client.ExecuteGet<F.Topic>("/" + topic.Id);
            
            mockedService.Verify(r => r.Get(topic.Id.Value));

            Assert.AreEqual(topic.Description, result.Description);
            Assert.AreEqual(TellagoStudios.Hermes.RestService.Constants.Relationships.Group, result.Group.rel);
            Assert.AreEqual(ResourceLocation.OfGroup(topic.GroupId), result.Group.href);
            Assert.AreEqual(topic.Id, result.Id.ToModel());
            Assert.AreEqual(topic.Name, result.Name);
        }

       [Test]
       public void Validates_a_get_with_an_invalid_id()
        {
            var id = Identity.Random();
            mockedService.Setup(r => r.Get(It.IsAny<Identity>())).Throws<EntityNotFoundException>();

            var result = client.ExecuteGet<F.Topic>("/" + id, HttpStatusCode.NotFound);

            mockedService.Verify(r => r.Get(id));

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
            mockedService.Setup(r => r.Find(null, null, null)).Returns(topics);

            var result = client.ExecuteGet<F.Topic[]>("");

            mockedService.Verify(r => r.Find(null, null, null));

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

            mockedService.Setup(r => r.Find(query, skip, limited)).Returns(new M.Topic[0]);

            var result = client.ExecuteGet<F.Topic[]>("?query=" + query);

            mockedService.Verify(r => r.Find(query, skip, limited));
        }

        [Test]
        public void Should_get_all_topic_paged()
        {
            string query = null;
            int? skip = 2;
            int? limited = 10;
            mockedService.Setup(r => r.Find(query, skip, limited)).Returns(new M.Topic[0]);

            var result = client.ExecuteGet<F.Topic[]>("?skip=" + skip.ToString() + "&limit=" + limited.ToString());

            mockedService.Verify(r => r.Find(query, skip, limited));
        }
        
        [Test]
        public void Should_get_all_topic_filtered_and_paged()
        {
            string query = "foo query";
            int? skip = 2;
            int? limited = 10;
            mockedService.Setup(r => r.Find(query, skip, limited)).Returns(new M.Topic[0]);

            var result = client.ExecuteGet<F.Topic[]>("?query=" + query + "&skip=" + skip.ToString() + "&limit=" + limited.ToString());

            mockedService.Verify(r => r.Find(query, skip, limited));
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

            var group = new M.Group { Id = M.Identity.Random() };
            var topic = new M.Topic()
            {
                Description = topicPost.Description,
                GroupId = group.Id.Value,
                Id = Identity.Random(),
                Name = topicPost.Name
            };


            mockedService.Setup(r => r.Create(It.IsAny<M.Topic>())).Returns(topic);

            var result = client.ExecutePost<F.TopicPost, F.Topic>("", topicPost);

            mockedService.Verify(r => r.Create(It.Is<M.Topic>(t => t!=null)));
            mockedService.Verify(r => r.Create(It.Is<M.Topic>(t => t.Description == topicPost.Description)));
            mockedService.Verify(r => r.Create(It.Is<M.Topic>(t => t.Name == topicPost.Name)));
            mockedService.Verify(r => r.Create(It.Is<M.Topic>(t => t.GroupId == topicPost.GroupId.ToModel())));

            Assert.AreEqual(topic.Description, result.Description);
            Assert.AreEqual(TellagoStudios.Hermes.RestService.Constants.Relationships.Group, result.Group.rel);
            Assert.AreEqual(ResourceLocation.OfGroup(topic.GroupId), result.Group.href);
            Assert.AreEqual(topic.Id, result.Id.ToModel());
            Assert.AreEqual(topic.Name, result.Name);
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


            mockedService.Setup(r => r.Update(It.IsAny<M.Topic>())).Returns(topic);

            var result = client.ExecutePut<F.TopicPut, F.Topic>("", topicPut);

            mockedService.Verify(r => r.Update(It.Is<M.Topic>(t => t != null)));
            mockedService.Verify(r => r.Update(It.Is<M.Topic>(t => t.Description == topicPut.Description)));
            mockedService.Verify(r => r.Update(It.Is<M.Topic>(t => t.Name == topicPut.Name)));
            mockedService.Verify(r => r.Update(It.Is<M.Topic>(t => t.GroupId == topicPut.GroupId.ToModel())));

            Assert.AreEqual(topic.Description, result.Description);
            Assert.AreEqual(TellagoStudios.Hermes.RestService.Constants.Relationships.Group, result.Group.rel);
            Assert.AreEqual(ResourceLocation.OfGroup(topic.GroupId), result.Group.href);
            Assert.AreEqual(topic.Id, result.Id.ToModel());
            Assert.AreEqual(topic.Name, result.Name);
        }

        [Test]
        public void Should_delete_a_topic()
        {
            var topicId = Identity.Random();

            client.ExecuteDelete("/" + topicId);

            mockedService.Verify(r => r.Delete(topicId));
        }
    }
}
