using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Topics.Commands
{
    public class DeleteTopicCommand : IDeleteTopicCommand
    {
        private readonly IEntityById entityById;
        private readonly ICudOperations<Topic> cudOperations;

        public DeleteTopicCommand(
            IEntityById entityById, 
            ICudOperations<Topic> cudOperations)
        {
            this.entityById = entityById;
            this.cudOperations = cudOperations;
        }

        public void Execute(Topic topic)
        {
            if(!topic.Id.HasValue) throw new ValidationException(Messages.IdMustNotBeNull);
            if (!entityById.Exist<Topic>(topic.Id.Value)) throw new EntityNotFoundException(typeof (Topic), topic.Id.Value);

            cudOperations.MakeTransient(topic);
        }
    }
}