using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public class UpdateGroupCommand : IUpdateGroupCommand
    {
        private readonly IEntityById entityById;
        private readonly IRepository<Group> repository;
        private readonly IExistGroupByGroupName existGroupByGroupName;

        public UpdateGroupCommand(IExistGroupByGroupName existGroupByGroupName, IEntityById entityById, IRepository<Group> repository) 
        {
            this.existGroupByGroupName = existGroupByGroupName;
            this.entityById = entityById;
            this.repository = repository;
        }

        public void Execute(Group group)
        {
            if (!group.Id.HasValue) throw new ValidationException(Messages.IdMustNotBeNull);
            if (!entityById.Exist<Group>(group.Id.Value)) throw new EntityNotFoundException(typeof(Group), group.Id.Value);
            if (string.IsNullOrWhiteSpace(group.Name)) throw new ValidationException(Messages.NameMustBeNotNull);
            if (existGroupByGroupName.Execute(group.Name, group.Id)) throw new ValidationException(Messages.GroupNameMustBeUnique);
            if (group.ParentId.HasValue && !entityById.Exist<Group>(group.ParentId.Value)) throw new ValidationException(Messages.EntityNotFound);

            ValidateCircleReferences(group);
            repository.Update(group);
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