using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface IMessageRepository
    {
        Message Get(MessageKey key);
        Message Create(Message message);
        IEnumerable<MessageKey> GetMessageKeys(Identity topicId, string query = null);
        bool Exists(MessageKey key, string filter);
    }
}