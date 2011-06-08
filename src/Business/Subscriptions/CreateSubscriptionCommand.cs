using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Subscriptions
{
    public class CreateSubscriptionCommand : ICreateSubscriptionCommand
    {
        private readonly IQueryValidator queryValidator;
        private readonly IEntityById entityById;
        private readonly IRepository<Subscription> repository;

        public CreateSubscriptionCommand(
            IQueryValidator queryValidator, 
            IEntityById entityById, 
            IRepository<Subscription> repository)
        {
            this.queryValidator = queryValidator;
            this.entityById = entityById;
            this.repository = repository;
        }

        public void Execute(Subscription subscription)
        {
            if (!queryValidator.IsValid(subscription.Filter)) throw new ValidationException(string.Format(Texts.InvalidFilter, subscription.Filter));
            if (subscription.TargetId == null) throw new ValidationException(Texts.TargetIdMustNotBeNull);
            if (!entityById.Exist<Topic>(subscription.TargetId.Value)) throw new EntityNotFoundException(typeof(Topic), subscription.TargetId.Value);
            repository.MakePersistent(subscription);
        }
    }
}