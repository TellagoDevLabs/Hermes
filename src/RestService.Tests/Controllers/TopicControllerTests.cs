using NUnit.Framework;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Controllers;

namespace RestService.Tests.Controllers
{
    [TestFixture]
    public class TopicControllerTests
    {
        [Test]
        public void CanExecuteIndex()
        {
            var topicsController = new TopicController(new RepositoryStub<Topic>())
        }
    }
}