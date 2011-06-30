using System.IO;
using System.Text;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Client.Tests.Util;

namespace TellagoStudios.Hermes.Client.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    public class MessagesDemo : IntegrationTestBase
    {
        private readonly HermesClient client = new HermesClient("http://localhost:40403");
        private Topic topic;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            topic = client.CreateGroup("Group1")
                          .CreateTopic("Topic1");
        }

        #endregion

        [Test]
        public void CanPostMessageToTopic()
        {
            var messageId = topic.PostMessage(new MemoryStream(Encoding.UTF8.GetBytes("Hello world")));
            messageId.Should().Not.Be.NullOrEmpty();
        }
    }
}