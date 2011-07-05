using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Client.Tests.Util;

namespace TellagoStudios.Hermes.Client.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    public class TopicsDemo : IntegrationTestBase
    {
        private readonly HermesClient client = new HermesClient("http://localhost:40403");
        private Group sampleGroup;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            sampleGroup = client.CreateGroup("Sample Group");
        }

        #endregion
        

        [Test]
        public void CanCreateATopic()
        {
            var topic = sampleGroup.CreateTopic("Test topic", "Test description");

            topic.Satisfy(t => t.Name == "Test topic" 
                       && t.Description == "Test description"
                       && !string.IsNullOrEmpty(t.Id));
        }
        
        [Test]
        public void WhenCallingTryCreateTopicWithTopicNameThatAlreadyExistThenReturnTopic()
        {
            var topic = sampleGroup.CreateTopic("Test topic", "Test description");
            var tryTopic = sampleGroup.TryCreateTopic("Test topic", "Test description");
            tryTopic.Should().Be.EqualTo(topic);
        }

        [Test]
        public void CanGetAllTopicsForAGroup()
        {
            var topic1 = sampleGroup.CreateTopic("Topic1");
            var topic2 = sampleGroup.CreateTopic("Topic2");
            var topic3 = client.CreateGroup("AnotherGroup").CreateTopic("Topic3");

            sampleGroup.GetTopics()
                .Should()
                        .Contain(topic1)
                        .And.Contain(topic2)
                        .And.Not.Contain(topic3);
        }


        [Test]
        public void CanDeleteATopic()
        {
            var topic1 = sampleGroup.CreateTopic("Topic1");
            
            topic1.Delete();

            sampleGroup.GetTopics().Should().Be.Empty();
        }

        [Test]
        public void CanUpdateTopic()
        {
            var topic1 = sampleGroup.CreateTopic("Topic1");

            topic1.Name = "FooBarBaz";
            topic1.SaveChanges();

            sampleGroup.GetTopics().Satisfy(
                ts => !ts.Any(t => t.Name == "Topic1") 
                    && ts.Any(t => t.Name == "FooBarBaz"));
        }
    }
}