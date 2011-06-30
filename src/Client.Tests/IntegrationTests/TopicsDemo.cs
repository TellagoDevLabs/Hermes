using System;
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
        public void WhenTopicIsCreated_ThenIsPersistedShouldBeTrue()
        {
            var topic = sampleGroup.CreateTopic("Test topic", "Test description");

            topic.Satisfy(t => t.Name == "Test topic" 
                       && t.Description == "Test description"
                       && !string.IsNullOrEmpty(t.Id));
        }

        //[Test]
        //public void WhenCreatingATopicWithinTransientGroup_ThenPersistGroupToo()
        //{
        //    var group1 = new Group("FooBarGroup");
        //    var topic = new Topic("Test topic", group1);
        //    client.CreateTopic(topic);
        //    group1.IsPersisted.Should().Be.True();
        //}

        //[Test]
        //public void WhenCreatingTwoTopicsWithSameNameWithinSameGroup_ThenFail()
        //{
        //    var topic1 = new Topic("Test topic", sampleGroup);
        //    var topic2 = new Topic("Test topic", sampleGroup);

        //    client.CreateTopic(topic1);

        //    client.Executing(c => c.CreateTopic(topic2))
        //        .Throws<WebException>();

        //}

        //[Test]
        //public void WhenCallingCreateWithAPersistedTopic_ThenFail()
        //{
        //    var topic = new Topic("Test topic", sampleGroup);

        //    client.CreateTopic(topic);

        //    client.Executing(c => c.CreateTopic(topic))
        //        .Throws<InvalidOperationException>();
        //}

        //[Test]
        //public void CanDeleteTopic()
        //{
        //    var topic = new Topic("Test topic", sampleGroup);

        //    client.CreateTopic(topic);

        //    client.DeleteTopic(topic.Id);
        //}


        //[Test]
        //public void CanGetTopicById()
        //{
        //    var topic1 = new Topic("Topic 1", sampleGroup);
        //    var topic2 = new Topic("Topic 2", sampleGroup);
        //    var topic3 = new Topic("Topic 2", new Group("Another group"));

        //    client.CreateTopic(topic1);
        //    client.CreateTopic(topic2);
        //    client.CreateTopic(topic3);

        //    client.GetTopicsByGroup(sampleGroup.Id)
        //        .Should()
        //            .Contain(topic1)
        //            .And.Contain(topic2)
        //            .And.Not.Contain(topic3);
        //}
    }
}