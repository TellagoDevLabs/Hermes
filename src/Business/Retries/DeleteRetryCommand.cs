using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Retries 
{
    public class DeleteRetryCommand : IDeleteRetryCommand
    {
        private readonly IEntityById entityById;
        private readonly IRepository<Retry> repository;

        public DeleteRetryCommand(
            IEntityById entityById, 
            IRepository<Retry> repository)
        {
            this.entityById = entityById;
            this.repository = repository;
        }

        public void Execute(Identity id)
        {
            if (!entityById.Exist<Retry>(id)) throw new EntityNotFoundException(typeof (Retry), id);

            repository.MakeTransient(id);
        }
    }
}