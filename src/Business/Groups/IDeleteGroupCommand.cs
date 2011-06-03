using System;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public interface IDeleteGroupCommand
    {
        void Execute(Group group);
    }

    public class DeleteGroupCommand : IDeleteGroupCommand
    {
        private readonly IQueryEntityById queryEntityById;
        private readonly ICudOperations<Group> cudOperations;
        private readonly IChildGroupsOfGroup childGroupsOfGroup;
        private readonly ITopicsByGroup topicsByGroup;

        public DeleteGroupCommand(
                IQueryEntityById queryEntityById, 
                ICudOperations<Group> cudOperations, 
                IChildGroupsOfGroup childGroupsOfGroup, 
                ITopicsByGroup topicsByGroup)
        {
            this.queryEntityById = queryEntityById;
            this.cudOperations = cudOperations;
            this.childGroupsOfGroup = childGroupsOfGroup;
            this.topicsByGroup = topicsByGroup;
        }

        public void Execute(Group group)
        {
            if(!group.Id.HasValue)
            {
                throw new ValidationException(Messages.IdMustNotBeNull);
            }
            if (!queryEntityById.Exist<Group>(group.Id.Value))
            {
                throw new EntityNotFoundException(typeof (Group), group.Id.Value);
            }
            if(childGroupsOfGroup.HasChilds(group))
            {
                throw new ValidationException(string.Format(Messages.GroupContainsChildGroups, group.Id));
            }
            if (topicsByGroup.HasTopics(group))
            {
                throw new ValidationException(string.Format(Messages.GroupContainsChildTopics, group.Id));
            }
            cudOperations.MakeTransient(group);
        }
    }
}