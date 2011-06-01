using System;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public interface ICreateGroupCommand
    {
        void Create(Group group);
    }

    public class CreateGroupCommand : ICreateGroupCommand
    {
        private readonly IExistGroupByGroupName existGroupByGroupName;
        private readonly IExistEntityById existEntityById;

        public CreateGroupCommand(IExistGroupByGroupName existGroupByGroupName, IExistEntityById existEntityById)
        {
            this.existGroupByGroupName = existGroupByGroupName;
            this.existEntityById = existEntityById;
        }

        public void Create(Group group)
        {
            if (group.Name == null) throw new ValidationException(Messages.NameMustBeNotNull);
            if(existGroupByGroupName.Execute(group.Name)) throw new ValidationException(Messages.GroupNameMustBeUnique);
            if(group.ParentId.HasValue && !existEntityById.Execute<Group>(group.ParentId.Value)) throw new ValidationException(Messages.EntityNotFound);

        }
    }
}