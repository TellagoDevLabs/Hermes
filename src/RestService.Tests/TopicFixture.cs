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
    public class TopicFixture : ResourceBaseFixture
    {
        private Mock<IUpdateTopicCommand> mockedUpdateCommand;
        private Mock<IDeleteTopicCommand> mockedDeleteCommand;
        private Mock<IEntityById> mockedEntityById;
        private Mock<ITopicByName> mockedTopicByName;

        protected override void PopulateApplicationContext(ContainerBuilder builder)
        {
            // Create a mocked repository for topics.
            mockedUpdateCommand = new Mock<IUpdateTopicCommand>();
            mockedDeleteCommand = new Mock<IDeleteTopicCommand>();
            mockedEntityById = new Mock<IEntityById>();
            mockedTopicByName = new Mock<ITopicByName>();

            builder.RegisterInstance(new TopicResource(
                mockedEntityById.Object,
                mockedUpdateCommand.Object,
                mockedDeleteCommand.Object,
                mockedTopicByName.Object));
        }

        protected override RestClient.SerializationType GetSerializationType()
        {
            return RestClient.SerializationType.Xml;
        }

        protected override Type GetServiceType()
        {
            return typeof(TopicResource);
        }

        [Test]
        public void Should_get_a_topic_by_id()
        {
            var group = new M.Group { Id = Identity.Random() };
            var topic = new M.Topic()
            {
                Description = "description",
                GroupId = group.Id.Value,
                Id = Identity.Random(),
                Name = "test"
            };
            mockedEntityById.Setup(r => r.Get<Topic>(topic.Id.Value)).Returns(topic);

            var result = client.ExecuteGet<F.Topic>("/" + topic.Id);

            Assert.AreEqual(topic.Description, result.Description);
            Assert.AreEqual(topic.Id, result.Id.ToModel());
            Assert.AreEqual(topic.Name, result.Name);
        }

        [Test]
        public void Should_get_a_topic_by_name_and_groupId()
        {
            var group = new M.Group { Id = Identity.Random() };
            var topic = new M.Topic()
            {
                Description = "description",
                GroupId = group.Id.Value,
                Id = Identity.Random(),
                Name = "test"
            };
            mockedTopicByName.Setup(r => r.Get(topic.Name, group.Id)).Returns(topic);

            var result = client.ExecuteGet<F.Topic>("/?name=" + topic.Name + "&groupId=" + group.Id);

            Assert.AreEqual(topic.Description, result.Description);
            Assert.AreEqual(topic.Id, result.Id.ToModel());
            Assert.AreEqual(topic.Name, result.Name);
        }

        [Test]
        public void Should_get_a_topic_by_name_and_without_group()
        {
            var topic = new M.Topic()
            {
                Description = "description",
                Id = Identity.Random(),
                Name = "test"
            };
            mockedTopicByName.Setup(r => r.Get(topic.Name, null)).Returns(topic);

            var result = client.ExecuteGet<F.Topic>("/?name=" + topic.Name);

            Assert.AreEqual(topic.Description, result.Description);
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
        public void Should_put_a_topic()
        {
            var topicPut = new F.TopicPut()
            {
                Id = F.Identity.Random(),
                Description = "description",
                GroupId = F.Identity.Random(),
                Name = "test"
            };

            mockedUpdateCommand.Setup(r => r.Execute(It.IsAny<M.Topic>()));

            client.ExecutePut(topicPut.Id.ToString(), topicPut);

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
