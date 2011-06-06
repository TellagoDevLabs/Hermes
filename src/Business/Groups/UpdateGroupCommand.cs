using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public class UpdateGroupCommand : ChangeGroupCommandBase, IUpdateGroupCommand
    {
        private readonly IEntityById entityById;
        private readonly ICudOperations<Group> cudOperations;

        public UpdateGroupCommand(IExistGroupByGroupName existGroupByGroupName, IEntityById entityById, ICudOperations<Group> cudOperations) 
            : base(existGroupByGroupName, entityById)
        {
            this.entityById = entityById;
            this.cudOperations = cudOperations;
        }

        public override void Execute(Group group)
        {
            if (!group.Id.HasValue) throw new ValidationException(Messages.IdMustNotBeNull);
            if (!entityById.Exist<Group>(group.Id.Value)) throw new EntityNotFoundException(typeof(Group), group.Id.Value);

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
                currentParentId = entityById.Get<Group>(currentParentId.Value).ParentId;
            }

            if (parentsId.Contains(group.Id.Value))
                throw new ValidationException(string.Format(Messages.GroupCircleReference, group.Id));
        }
    }
}