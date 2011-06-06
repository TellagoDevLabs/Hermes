using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.Business.Topics;
using TellagoStudios.Hermes.Business.Topics.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public class CreateTopicCommand : ICreateTopicCommand
    {
        private readonly IEntityById entityById;
        private readonly IExistsTopicByName existsTopicByName;
        private readonly ICudOperations<Topic> cudOperations;

        public CreateTopicCommand(
            IExistsTopicByName existsTopicByName, 
            IEntityById entityById,
            ICudOperations<Topic> cudOperations)
        {
            this.entityById = entityById;
            this.existsTopicByName = existsTopicByName;
            this.cudOperations = cudOperations;
        }

        public virtual void Execute(Topic topic)
        {
            if (string.IsNullOrWhiteSpace(topic.Name )) throw new ValidationException(Messages.NameMustBeNotNull);
            if (existsTopicByName.Execute(topic.Name)) throw new ValidationException(Messages.TopicNameMustBeUnique, topic.Name);
            if (!entityById.Exist<Group>(topic.GroupId)) throw new ValidationException(Messages.EntityNotFound, typeof(Group).Name, topic.GroupId);

            cudOperations.MakePersistent(topic);
        }
    }
}