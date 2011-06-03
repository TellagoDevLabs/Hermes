using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public interface IUpdateGroupCommand
    {
        void Execute(Group group);
    }

    public class UpdateGroupCommand : ChangeGroupCommandBase, IUpdateGroupCommand
    {
        private readonly IQueryEntityById queryEntityById;
        private readonly ICudOperations<Group> cudOperations;

        public UpdateGroupCommand(IExistGroupByGroupName existGroupByGroupName, IQueryEntityById queryEntityById, ICudOperations<Group> cudOperations) 
            : base(existGroupByGroupName, queryEntityById)
        {
            this.queryEntityById = queryEntityById;
            this.cudOperations = cudOperations;
        }

        public override void Execute(Group group)
        {
            base.Execute(group);
            ValidateCircleReferences(group);
            cudOperations.Update(group);
        }

        private void ValidateCircleReferences(Group group)
        {
            var parentsId = new HashSet<Identity>();
            var currentParentId = group.ParentId;
            while (currentParentId.HasValue)
            {
                if(!parentsId.Add(currentParentId.Value)) break;
                currentParentId = queryEntityById.Get<Group>(currentParentId.Value).ParentId;
            }

            if (parentsId.Contains(group.Id.Value))
                throw new ValidationException(string.Format(Messages.GroupCircleReference, group.Id));
        }
    }
}