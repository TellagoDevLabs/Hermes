using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Topics    
{
    public class CreateTopicCommand : ICreateTopicCommand
    {
        private readonly IEntityById entityById;
        private readonly IExistsTopicByName existsTopicByName;
        private readonly IRepository<Topic> repository;

        public CreateTopicCommand(
            IExistsTopicByName existsTopicByName, 
            IEntityById entityById,
            IRepository<Topic> repository)
        {
            this.entityById = entityById;
            this.existsTopicByName = existsTopicByName;
            this.repository = repository;
        }

        public virtual void Execute(Topic topic)
        {
            if (string.IsNullOrWhiteSpace(topic.Name )) throw new ValidationException(Texts.NameMustBeNotNull);
            if (existsTopicByName.Execute(topic.Name)) throw new ValidationException(Texts.TopicNameMustBeUnique, topic.Name);
            if (!entityById.Exist<Group>(topic.GroupId)) throw new ValidationException(Texts.EntityNotFound, typeof(Group).Name, topic.GroupId);

            repository.MakePersistent(topic);
        }
    }
}