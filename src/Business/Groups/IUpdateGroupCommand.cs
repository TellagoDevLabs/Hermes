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
        private readonly ICudOperations<Group> cudOperations;

        public UpdateGroupCommand(IExistGroupByGroupName existGroupByGroupName, IQueryEntityById queryEntityById, ICudOperations<Group> cudOperations) 
            : base(existGroupByGroupName, queryEntityById)
        {
            this.cudOperations = cudOperations;
        }

        public override void Execute(Group group)
        {
            base.Execute(group);
            if (group.ParentId.HasValue && group.ParentId == group.Id)
                throw new ValidationException(string.Format(Messages.GroupCircleReference, group.Id));
            cudOperations.Update(group);
        }
    }
}