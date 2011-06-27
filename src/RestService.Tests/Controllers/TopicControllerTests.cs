using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using RestService.Tests.Util;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
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
            IGroupsSortedByName groupsSortedByName = null)
        {
            var defaultGroupQuery = Mock.Of<IGroupsSortedByName>(q => q.Execute() == new Group[]{SampleGroup});
            return new TopicController( topicsSortedByName ?? Mock.Of<ITopicsSortedByName>(),
                                        groupsSortedByName ?? defaultGroupQuery, 
                                        entityById ?? Mock.Of<IEntityById>());
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
            var expected = new Topic {Name = "Topic 1", Description = "Foo", GroupId = SampleGroup.Id.Value};
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


    }
}