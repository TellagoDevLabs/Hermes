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
            if (!topic.Id.HasValue) throw new ValidationException(Texts.IdMustNotBeNull);
            if (!entityById.Exist<Topic>(topic.Id.Value)) throw new EntityNotFoundException(typeof(Topic), topic.Id.Value);
            if (string.IsNullOrWhiteSpace(topic.Name )) throw new ValidationException(Texts.NameMustBeNotNull);
            if (existsTopicByName.Execute(topic.GroupId, topic.Name, topic.Id)) throw new ValidationException(Texts.TopicNameMustBeUnique, topic.Name);
            if (topic.GroupId.HasValue && !entityById.Exist<Group>(topic.GroupId.Value)) throw new ValidationException(Texts.EntityNotFound, typeof(Group).Name, topic.GroupId);

            repository.Update(topic);
        }
    }
}