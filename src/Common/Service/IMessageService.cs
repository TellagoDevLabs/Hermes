using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Service
{
    public interface IMessageService
    {
        Message Create(Message message);
        IEnumerable<Message> CreateByGroup(Guid groupId, Message message);
        IEnumerable<MessageKey> GetMessageKeysBySubscription(Guid subscriptionId);
        Message Get(MessageKey key);
        IEnumerable<MessageKey> GetForTopic(Guid topicId);
        IEnumerable<MessageKey> GetForGroup(Guid groupId);
        bool Exists(MessageKey key, string filter);
    }
}