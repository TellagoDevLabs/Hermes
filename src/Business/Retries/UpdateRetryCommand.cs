using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Retries
{
    public class UpdateRetryCommand : IUpdateRetryCommand
    {
        private readonly IEntityById entityById;
        private readonly IRepository<Retry> repository;

        public UpdateRetryCommand(
            IEntityById entityById,
            IRepository<Retry> repository)
        {
            this.entityById = entityById;
            this.repository = repository;
        }

        public virtual void Execute(Retry retry)
        {
            if (!entityById.Exist<Retry>(retry.Id.Value)) throw new EntityNotFoundException(typeof(Retry), retry.Id.Value);

            repository.Update(retry);
        }
    }
}