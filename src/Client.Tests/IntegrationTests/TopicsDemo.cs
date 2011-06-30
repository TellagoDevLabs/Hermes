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
        public void CanGetAllTopicsForAGroup()
        {
            var topic1 = sampleGroup.CreateTopic("Topic1");
            var topic2 = sampleGroup.CreateTopic("Topic2");
            sampleGroup.GetAllTopics()
                .Should().Have.SameValuesAs(topic1, topic2);
        }


        [Test]
        public void CanDeleteATopic()
        {
            var topic1 = sampleGroup.CreateTopic("Topic1");
            
            topic1.Delete();

            sampleGroup.GetAllTopics().Should().Be.Empty();
        }

        [Test]
        public void CanUpdateTopic()
        {
            var topic1 = sampleGroup.CreateTopic("Topic1");

            topic1.Name = "FooBarBaz";
            topic1.SaveChanges();

            sampleGroup.GetAllTopics().Satisfy(
                ts => !ts.Any(t => t.Name == "Topic1") 
                    && ts.Any(t => t.Name == "FooBarBaz"));
        }
    }
}