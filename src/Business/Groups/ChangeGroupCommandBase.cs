using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public class ChangeGroupCommandBase : ICreateGroupCommand
    {
        private readonly IExistGroupByGroupName existGroupByGroupName;
        private readonly IQueryEntityById queryEntityById;

        public ChangeGroupCommandBase(IExistGroupByGroupName existGroupByGroupName, IQueryEntityById queryEntityById)
        {
            this.existGroupByGroupName = existGroupByGroupName;
            this.queryEntityById = queryEntityById;
        }

        public virtual void Execute(Group group)
        {
            if (group.Name == null) throw new ValidationException(Messages.NameMustBeNotNull);
            if (existGroupByGroupName.Execute(group.Name, group.Id)) throw new ValidationException(Messages.GroupNameMustBeUnique);
            if (group.ParentId.HasValue && !queryEntityById.Exist<Group>(group.ParentId.Value)) throw new ValidationException(Messages.EntityNotFound);
        }
    }
}