using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;

namespace TellagoStudios.Hermes.Business.Retries
{
    public class CreateRetryCommand : ICreateRetryCommand
    {
        private readonly IRepository<Retry> repository;

        public CreateRetryCommand(IRepository<Retry> repository)
        {
            this.repository = repository;
        }

        public virtual void Execute(Retry retry)
        {
            repository.MakePersistent(retry);
        }
    }
}