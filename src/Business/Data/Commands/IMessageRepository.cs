using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Commads
{
    public interface IMessageRepository 
    {
        void MakePersistent(Message message);
        void MakeTransient(MessageKey key);
        void Update(Message message);
    }
}