using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace TellagoStudios.Hermes.Business.Groups
{
    public class CreateGroupCommand : ChangeGroupCommandBase, ICreateGroupCommand
    {
        private readonly ICudOperations<Group> cudOperations;

        public CreateGroupCommand(
            IExistGroupByGroupName existGroupByGroupName, 
            IEntityById entityById, 
            ICudOperations<Group> cudOperations)
            : base(existGroupByGroupName, entityById)
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