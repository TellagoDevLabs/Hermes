using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Subscriptions
{
    public class DeleteSubscriptionCommand : IDeleteSubscriptionCommand
    {
        private readonly IEntityById entityById;
        private readonly IRepository<Subscription> repository;

        public DeleteSubscriptionCommand(IEntityById entityById, IRepository<Subscription> repository)
        {
            this.entityById = entityById;
            this.repository = repository;
        }

        public void Execute(Identity identity)
        {
            if(!entityById.Exist<Subscription>(identity)) throw new EntityNotFoundException(typeof(Subscription), identity);
            repository.MakeTransient(identity);
        }
    }
}