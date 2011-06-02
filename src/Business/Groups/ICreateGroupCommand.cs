using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public interface ICreateGroupCommand
    {
        void Execute(Group group);
    }

    public class CreateGroupCommand : ChangeGroupCommandBase
    {
        private readonly ICudOperations<Group> cudOperations;

        public CreateGroupCommand(
            IExistGroupByGroupName existGroupByGroupName, 
            IQueryEntityById queryEntityById, 
            ICudOperations<Group> cudOperations)
            : base(existGroupByGroupName, queryEntityById)
        {
            this.cudOperations = cudOperations;
        }

        public override void Execute(Group group)
        {
            base.Execute(group);
            cudOperations.MakePersistent(group);
        }
    }
}