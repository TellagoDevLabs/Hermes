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

        public CreateGroupCommand(IExistGroupByGroupName existGroupByGroupName)
        {
            this.existGroupByGroupName = existGroupByGroupName;
        }

        public void Create(Group group)
        {
            if (group.Name == null) throw new ValidationException(Messages.NameMustBeNotNull);
            if(existGroupByGroupName.Execute(group.Name)) throw new ValidationException(Messages.GroupNameMustBeUnique);
        }
    }
}