using System;
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
    public class CreateSubscriptionCommandTests
    {
        public CreateSubscriptionCommand CreateCommand(
            IQueryValidator isQueryValid = null, 
            IEntityById entityById = null, 
            IRepository<Subscription> repository  = null)
        {
            return new CreateSubscriptionCommand(
                isQueryValid ?? Mock.Of<IQueryValidator>(qv => qv.IsValid(It.IsAny<string>())),
                entityById ?? Mock.Of<IEntityById>(eb => eb.Exist<Topic>(It.IsAny<Identity>())),
                repository ?? Mock.Of<IRepository<Subscription>>());    
        }

        [Test]
        public void WhenQueryIsNotValid_ThenThrowException()
        {
            var command = CreateCommand(isQueryValid: Mock.Of<IQueryValidator>(qv => qv.IsValid("pp") == false));
            command.Executing(c => c.Execute(new Subscription{Filter = "pp"}))
                   .Throws<ValidationException>()
                   .And
                   .Exception.Message.Should().Be.EqualTo(string.Format(Texts.InvalidFilter, "pp"));
        }

        [Test]
        public void WhenTargetIdIsNull_ThenThrowValidationException()
        {
            var command = CreateCommand();
            command.Executing(c => c.Execute(new Subscription { TargetId = null }))
                   .Throws<ValidationException>()
                   .And
                   .Exception.Message.Should().Be.EqualTo(Texts.TargetIdMustNotBeNull);
        }

        [Test]
        public void WhenTargetTopicDoesNotExists_ThenThrowValidationException()
        {
            var command = CreateCommand(entityById: Mock.Of<IEntityById>(ebi => ebi.Exist<Topic>(It.IsAny<Identity>()) == false));
            var targetId = Identity.Random();
            command.Executing(c => c.Execute(new Subscription { TargetId = targetId }))
                   .Throws<EntityNotFoundException>()
                   .And
                   .Exception.Message.Should().Be.EqualTo(string.Format(Texts.EntityNotFound, typeof(Topic).Name, targetId));
        }

        [Test]
        public void WhenAllIsOk_ThenInsert()
        {
            var stubRepository = new StubRepository<Subscription>();
            var command = CreateCommand(repository: stubRepository);
            var targetId = Identity.Random();
            var subscription = new Subscription { TargetId = targetId };
            command.Execute(subscription);

            stubRepository.Entities.Should().Contain(subscription);

        }
        
    }
}