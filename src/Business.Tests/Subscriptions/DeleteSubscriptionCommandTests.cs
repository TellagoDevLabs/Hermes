using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Subscriptions;

namespace Business.Tests.Subscriptions
{
    [TestFixture]
    public class DeleteSubscriptionCommandTests
    {
        [Test]
        public void WhenEntityDoesNotExists_ThenThrowEntityNotFound()
        {
            var entityById = Mock.Of<IEntityById>(eb => eb.Exist<Subscription>(It.IsAny<Identity>()) == false);
            var command = new DeleteSubscriptionCommand(entityById, Mock.Of<IRepository<Subscription>>());
            var identity = Identity.Random();
            command.Executing(c => c.Execute(identity))
                    .Throws<EntityNotFoundException>();
        }

        [Test]
        public void WhenAllIsOk_ThenDelete()
        {
            var identity = Identity.Random();
            var entityById = Mock.Of<IEntityById>(eb => eb.Exist<Subscription>(It.IsAny<Identity>()) == true);
            var repository = new StubRepository<Subscription>(new Subscription{Id = identity});
            var command = new DeleteSubscriptionCommand(entityById, repository);

            
            command.Execute(identity);


            repository.Entities.Should().Be.Empty();
        }
    }
}