using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Messages
{
    public interface ICreateMessageCommand
    {
        void Execute(Message message);
    }
}