using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Topics
{
    public class UpdateTopicCommand : IUpdateTopicCommand
    {
        private readonly IEntityById entityById;
        private readonly IExistsTopicByName existsTopicByName;
        private readonly IRepository<Topic> repository;

        public UpdateTopicCommand(
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
            if (!topic.Id.HasValue) throw new ValidationException(Messages.IdMustNotBeNull);
            if (!entityById.Exist<Topic>(topic.Id.Value)) throw new EntityNotFoundException(typeof(Topic), topic.Id.Value);
            if (string.IsNullOrWhiteSpace(topic.Name )) throw new ValidationException(Messages.NameMustBeNotNull);
            if (existsTopicByName.Execute(topic.Name, topic.Id)) throw new ValidationException(Messages.TopicNameMustBeUnique, topic.Name);
            if (!entityById.Exist<Group>(topic.GroupId)) throw new ValidationException(Messages.EntityNotFound, typeof(Group).Name, topic.GroupId);

            repository.Update(topic);
        }
    }
}