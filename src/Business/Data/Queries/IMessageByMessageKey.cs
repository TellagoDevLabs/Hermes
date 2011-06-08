using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IMessageByMessageKey
    {
        bool Exist(MessageKey key);
        Message Get(MessageKey key);
    }
}