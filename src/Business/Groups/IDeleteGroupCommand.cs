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
        private readonly IQueryChildGroups queryChildGroups;

        public DeleteGroupCommand(IQueryEntityById queryEntityById, ICudOperations<Group> cudOperations, IQueryChildGroups queryChildGroups)
        {
            this.queryEntityById = queryEntityById;
            this.cudOperations = cudOperations;
            this.queryChildGroups = queryChildGroups;
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
            if(queryChildGroups.HasChilds(group))
            {
                throw new ValidationException(Messages.GroupContainsChildGroups);
            }
            cudOperations.MakeTransient(group);
        }
    }
}