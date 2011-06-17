using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Subscriptions;

namespace Business.Tests.Subscriptions
{
    [TestFixture]
    public class UpdateSubscriptionCommandTests
    {
        public UpdateSubscriptionCommand CreateCommand(
            IEntityById entityById = null,
            IQueryValidator queryValidator = null,
            IRepository<Subscription> repository = null
            )
        {
            return new UpdateSubscriptionCommand(
                entityById ?? Mock.Of<IEntityById>(eb => eb.Exist<Subscription>(It.IsAny<Identity>()) == true),
                queryValidator ?? Mock.Of<IQueryValidator>(qv => qv.IsValid(It.IsAny<string>()) == true),
                repository ?? Mock.Of<IRepository<Subscription>>()
                );
        }

        [Test]
        public void WhenIdIsNull_ThenThrowValidationException()
        {
            var command = CreateCommand();
            command.Executing(c => c.Execute(new Subscription {Id = null}))
                .Throws<ValidationException>()
                .And.Exception.Message.Should().Be.EqualTo(Texts.IdMustNotBeNull);
        }

        [Test]
        public void WhenEntitiyDoesNotExist_ThenThrowValidationException()
        {
            var command = CreateCommand(entityById: Mock.Of<IEntityById>(eb => eb.Exist<Subscription>(It.IsAny<Identity>()) == false));
            var subscription = new Subscription { Id = Identity.Random()};
            command.Executing(c => c.Execute(subscription))
                .Throws<EntityNotFoundException>();
        }
        
        [Test]
        public void WhenQueryIsInvalid_ThenThrowValidationException()
        {
            var command = CreateCommand(queryValidator: Mock.Of<IQueryValidator>(qv => qv.IsValid(It.IsAny<string>()) == false));
            
            var subscription = new Subscription { Id = Identity.Random(), Filter = "pp" };

            command.Executing(c => c.Execute(subscription))
                .Throws<ValidationException>()
                .And.Exception.Message.Should().Be.EqualTo(string.Format(Texts.InvalidFilter, "pp"));
        }

        [Test]
        public void WhenAllIsOk_ThenUpdate()
        {
            var repository = new StubRepository<Subscription>();
            var command = CreateCommand(repository: repository);
            var subscription = new Subscription { Id = Identity.Random(), Filter = "pp" };

            command.Execute(subscription);

            repository.Updates.Should().Contain(subscription);

        }
    }
}