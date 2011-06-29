using System;
using System.Net;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Client.Tests.Util;

namespace TellagoStudios.Hermes.Client.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    public class GroupsDemo : IntegrationTestBase
    {
        private readonly HermesClient client = new HermesClient("http://localhost:40403");

        [Test]
        public void WhenGroupIsCreated_ThenIsPersistedShouldBeTrue()
        {
            var group = new Group("TestGroup", "TestDescription");
            client.CreateGroup(group);
            group.IsPersisted.Should().Be.True();
        }


        [Test]
        public void WhenGroupIsCreated_ThenTheIdShouldNotBeEmpty()
        {
            var group = new Group("TestGroup", "TestDescription");
            client.CreateGroup(group);
            group.Id.Should().Not.Be.NullOrEmpty();
        }

        [Test]
        public void WhenCreatingTwoGroupsWithSameName_ThenFail()
        {
            var group1 = new Group("TestGroup", "TestDescription");
            client.CreateGroup(group1);

            var group2 = new Group("TestGroup", "TestDescription");
            client.Executing(c => c.CreateGroup(group2))
                  .Throws<WebException>();
        }

        [Test]
        public void WhenCallingCreateTwoTimesWithSameGroup_ThenFail()
        {
            var group = new Group("Test", "Foo");
            client.CreateGroup(group);
            client.Executing(c => c.CreateGroup(group))
                .Throws<InvalidOperationException>();
        }
    }
}
