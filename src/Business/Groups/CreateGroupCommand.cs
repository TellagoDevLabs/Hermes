using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public class CreateGroupCommand :  ICreateGroupCommand
    {
        private readonly IRepository<Group> repository;
        private readonly IExistGroupByGroupName existGroupByGroupName;
        private readonly IEntityById entityById;

        public CreateGroupCommand(
            IExistGroupByGroupName existGroupByGroupName, 
            IEntityById entityById, 
            IRepository<Group> repository)
        {
            this.existGroupByGroupName = existGroupByGroupName;
            this.entityById = entityById;
            this.repository = repository;
        }

        public void Execute(Group group)
        {
            if (string.IsNullOrWhiteSpace(group.Name)) throw new ValidationException(Messages.NameMustBeNotNull);
            if (existGroupByGroupName.Execute(group.Name)) throw new ValidationException(Messages.GroupNameMustBeUnique);
            if (group.ParentId.HasValue && !entityById.Exist<Group>(group.ParentId.Value)) throw new ValidationException(Messages.EntityNotFound);

            repository.MakePersistent(group);
        }
    }
}