using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Service
{
    public interface IMessageService
    {
        Message Create(Message message);
        IEnumerable<Message> CreateByGroup(Identity groupId, Message message);
        IEnumerable<MessageKey> GetMessageKeysBySubscription(Identity subscriptionId);
        Message Get(MessageKey key);
        IEnumerable<MessageKey> GetForTopic(Identity topicId);
        IEnumerable<MessageKey> GetForGroup(Identity groupId);
        bool Exists(MessageKey key, string filter);
    }
}