using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Topics
{
    public class DeleteTopicCommand : IDeleteTopicCommand
    {
        private readonly IEntityById entityById;
        private readonly IRepository<Topic> repository;

        public DeleteTopicCommand(
            IEntityById entityById, 
            IRepository<Topic> repository)
        {
            this.entityById = entityById;
            this.repository = repository;
        }

        public void Execute(Identity id)
        {
            if (!entityById.Exist<Topic>(id)) throw new EntityNotFoundException(typeof (Topic), id);

            repository.MakeTransient(id);
        }
    }
}