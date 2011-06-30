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
        public void CanGetGroups()
        {
            client.CreateGroup("Test", "Foo");
            client.GetGroups()
                  .Satisfy(gs => gs.Any(g => g.Name == "Test" && g.Description == "Foo")); 
        }

        [Test]
        public void CanDeleteGroup()
        {
            var group = client.CreateGroup("Test");
            group.Delete();

            client.GetGroups().Should().Be.Empty();
        }

        [Test]
        public void CanUpdateGroup()
        {
            var group = client.CreateGroup("Test");

            group.Name = "FooBar";
            group.SaveChanges();

            var groups = client.GetGroups();

            groups.Should().Have.Count.EqualTo(1);
            groups.First().Name.Should().Be.EqualTo("FooBar");

        }

        [Test]
        public void CanGetGroupById()
        {
            var group = client.CreateGroup("Test");
            var result = client.GetGroup(group.Id);

            result.Satisfy(r =>
                           r.Name == group.Name
                           && r.Description == group.Description);
        }
    }
}
