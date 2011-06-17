using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IMessageByMessageKey
    {
        bool Exist(MessageKey key);
        bool Exist(MessageKey key, string filter);
        Message Get(MessageKey key);
    }
}