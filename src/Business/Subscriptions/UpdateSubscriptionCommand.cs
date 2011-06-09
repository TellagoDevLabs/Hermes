using System;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Subscriptions
{
    public class UpdateSubscriptionCommand : IUpdateSubscriptionCommand
    {
        private readonly IEntityById entityById;
        private readonly IQueryValidator queryValidator;
        private readonly IRepository<Subscription> repository;

        public UpdateSubscriptionCommand(
            IEntityById entityById, 
            IQueryValidator queryValidator, 
            IRepository<Subscription> repository)
        {
            this.entityById = entityById;
            this.queryValidator = queryValidator;
            this.repository = repository;
        }

        public void Execute(Subscription subscription)
        {
            Validate(subscription);
            repository.Update(subscription);
        }

        private void Validate(Subscription subscription)
        {
            if(!subscription.Id.HasValue) throw new ValidationException(Texts.IdMustNotBeNull);
            if(!entityById.Exist<Subscription>(subscription.Id.Value)) throw new EntityNotFoundException(typeof(Subscription), subscription.Id.Value);
            if (string.IsNullOrWhiteSpace(subscription.Filter))
            {
                subscription.Filter = null;
            }
            else if (!queryValidator.IsValid(subscription.Filter))
            {
                throw new ValidationException(string.Format(Texts.InvalidFilter, subscription.Filter));
            }
        }
    }
}