using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public class DeleteGroupCommand : IDeleteGroupCommand
    {
        private readonly IEntityById entityById;
        private readonly IRepository<Group> repository;
        private readonly IChildGroupsOfGroup childGroupsOfGroup;
        private readonly ITopicsByGroup topicsByGroup;

        public DeleteGroupCommand(
            IEntityById entityById, 
            IRepository<Group> repository, 
            IChildGroupsOfGroup childGroupsOfGroup, 
            ITopicsByGroup topicsByGroup)
        {
            this.entityById = entityById;
            this.repository = repository;
            this.childGroupsOfGroup = childGroupsOfGroup;
            this.topicsByGroup = topicsByGroup;
        }

        public void Execute(Identity id)
        {
            if (!entityById.Exist<Group>(id))
            {
                throw new EntityNotFoundException(typeof (Group), id);
            }
            if(childGroupsOfGroup.HasChilds(id))
            {
                throw new ValidationException(string.Format(Messages.GroupContainsChildGroups, id));
            }
            if (topicsByGroup.HasTopics(id))
            {
                throw new ValidationException(string.Format(Messages.GroupContainsChildTopics, id));
            }
            repository.MakeTransient(id);
        }
    }
}