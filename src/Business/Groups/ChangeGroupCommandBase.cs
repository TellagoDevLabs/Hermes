using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public class ChangeGroupCommandBase
    {
        private readonly IExistGroupByGroupName existGroupByGroupName;
        private readonly IEntityById entityById;

        public ChangeGroupCommandBase(IExistGroupByGroupName existGroupByGroupName, IEntityById entityById)
        {
            this.existGroupByGroupName = existGroupByGroupName;
            this.entityById = entityById;
        }

        public virtual void Execute(Group group)
        {
            if (group.Name == null) throw new ValidationException(Messages.NameMustBeNotNull);
            if (existGroupByGroupName.Execute(group.Name, group.Id)) throw new ValidationException(Messages.GroupNameMustBeUnique);
            if (group.ParentId.HasValue && !entityById.Exist<Group>(group.ParentId.Value)) throw new ValidationException(Messages.EntityNotFound);
        }
    }
}