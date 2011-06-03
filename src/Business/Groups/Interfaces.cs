using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Groups
{
    public interface ICreateGroupCommand
    {
        void Execute(Group group);
    }

    public interface IUpdateGroupCommand
    {
        void Execute(Group group);
    }

    public interface IDeleteGroupCommand
    {
        void Execute(Group group);
    }
}