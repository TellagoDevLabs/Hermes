using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using RestService.Tests.Util;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Topics;
using TellagoStudios.Hermes.RestService.Controllers;
using TellagoStudios.Hermes.RestService.Models;

namespace RestService.Tests.Controllers
{
    [TestFixture]
    public class TopicControllerTests
    {
        private static readonly Group SampleGroup = new Group {Id = Identity.Random()};

        private static TopicController CreateController(
            ITopicsSortedByName topicsSortedByName = null, 
            IEntityById entityById = null,
            IGroupsSortedByName groupsSortedByName = null,
            ICreateTopicCommand createTopicCommand = null,
            IUpdateTopicCommand updateTopicCommand = null,
            IDeleteTopicCommand deleteTopicCommand = null)

        {
            var defaultGroupQuery = Mock.Of<IGroupsSortedByName>(q => q.Execute() == new Group[]{SampleGroup});
            return new TopicController( entityById ?? Mock.Of<IEntityById>(),
                                        topicsSortedByName ?? Mock.Of<ITopicsSortedByName>(),
                                        groupsSortedByName ?? defaultGroupQuery, 
                                        createTopicCommand ?? Mock.Of<ICreateTopicCommand>(),
                                        updateTopicCommand ?? Mock.Of<IUpdateTopicCommand>(),
                                        deleteTopicCommand ?? Mock.Of<IDeleteTopicCommand>()
                                        );
        }

        [Test]
        public void CanExecuteIndex()
        {
            var expected = new Topic[]{};
            var topicsController = CreateController(Mock.Of<ITopicsSortedByName>(q => q.Execute() == expected));

            topicsController.Index()
                    .GetModel<IEnumerable<Topic>>()
                    .Should().Be.SameInstanceAs(expected);
        }

        [Test]
        public void WhenTopicExist_ThenReturnViewResultWithTopicAsModel()
        {
            var expected = new Topic {Name = "Topic 1", Description = "Foo", GroupId = SampleGroup.Id.Value, Id = Identity.Random()};
            var entityById = Mock.Of<IEntityById>(e => e.Get<Topic>(It.IsAny<Identity>() ) == expected);
            
            var topicsController = CreateController(entityById: entityById);

            topicsController.Edit(Identity.Random().ToString())
                .GetModel<EditTopicModel>()
                .Satisfy(t => t.Name == "Topic 1" && t.Description == "Foo");
        }


        [Test]
        public void WhenTopicDoesNotExist_ThenReturn404()
        {
            var topicsController = CreateController();
            var result = topicsController.Edit(Identity.Random().ToString());
            result.Should().Be.OfType<HttpStatusCodeResult>()
                  .And.ValueOf.StatusCode.Should().Be.EqualTo(404);
        }


        [Test]
        public void WhenPostingATopic_ThenMapAndSaveChanges()
        {
            var topic = new Topic { Name = "Topic 1", Description = "Foo", GroupId = SampleGroup.Id.Value };
            var entityById = Mock.Of<IEntityById>(e => e.Get<Topic>(It.IsAny<Identity>()) == topic);

            var topicsController = CreateController(entityById: entityById);

            topicsController.Edit(new EditTopicModel {Description = "Desc", Name = "Tap"});
            topic.Satisfy(t => t.Description == "Desc" && t.Name == "Tap");
        }

        [Test]
        public void CreateMethod_ShouldReturnAModelWithAllGroups()
        {
            var controller = CreateController();
            controller.Create()
                      .GetModel<EditTopicModel>()
                      .Groups.Satisfy(gs => gs.Any(g => g.Name == SampleGroup.Name));
        }
    }
}