using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Repository
{
    public interface IMessageRepository
    {
        Message Get(MessageKey key);
        Message Create(Message message);
        IEnumerable<MessageKey> GetMessageKeys(Guid topicId, string query = null);
        //IEnumerable<MessageKey> GetForTopic(string topicId);
        bool Exists(MessageKey key, string filter);
    }
}