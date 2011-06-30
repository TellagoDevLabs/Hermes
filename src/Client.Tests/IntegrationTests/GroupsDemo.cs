using System.Linq;
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
        public void CanCreateGroup()
        {
            var group = client.CreateGroup("TestGroup", "TestDescription");
            group.Satisfy(g => g.Name == "TestGroup" && g.Description == "TestDescription");
        }

        [Test]
        public void CanCreateGroupWithoutDescription()
        {
            var group = client.CreateGroup("TestGroup");
            group.Satisfy(g => g.Name == "TestGroup" && string.IsNullOrEmpty(g.Description));
        }

        [Test]
        public void WhenCreatingTwoGroupsWithSameName_ThenFail()
        {
            client.CreateGroup("TestGroup", "TestDescription");
            client.Executing(c => c.CreateGroup("TestGroup", "TestDescription"))
                  .Throws<WebException>();
        }
        
        [Test]
        public void CanDeleteAGroup()
        {
            var group = client.CreateGroup("Test");
            client.DeleteGroup(group.Id);
        }

        [Test]
        public void CanGetAGroup()
        {
            client.CreateGroup("Test", "Foo");
            client.GetGroups()
                  .Satisfy(gs => gs.Any(g => g.Name == "Test" && g.Description == "Foo")); 
        }
    }
}
