using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Topics
{
    public interface ICreateTopicCommand
    {
        void Execute(Topic topic);
    }

    public interface IUpdateTopicCommand
    {
        void Execute(Topic topic);
    }

    public interface IDeleteTopicCommand
    {
        void Execute(Topic topic);
    }
}