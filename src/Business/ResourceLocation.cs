using System;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business
{
    public static class ResourceLocation
    {
        public static string OfTopic(Identity id)
        {
            return "/" + Constants.Routes.Topics + "/" + id;
        }

        public static string OfGroup(Identity id)
        {
            return "/" + Constants.Routes.Groups + "/" + id;
        }

        public static string OfMessageByTopic(Message message)
        {
            Guard.Instance
                .ArgumentNotNull(() => message, message)
                .ArgumentNotNull(() => message.Id, message.Id);

            return OfMessageByTopic(message.TopicId, message.Id.Value);
        }

        public static string OfMessageByTopic(MessageKey key)
        {
            Guard.Instance.ArgumentNotNull(()=>key, key);

            return OfMessageByTopic(key.TopicId, key.MessageId);
        }

        public static string OfMessageByTopic(Identity topicId, Identity messageId)
        {
            return "/" + Constants.Routes.Messages + "/" + messageId + "/topic/" + topicId;
        }

        public static string LinkToMessage(Identity topicId, Identity messageId)
        {
            return string.Format("<link rel=\"{0}\" href=\"{1}\"/>", Constants.Relationships.Message, OfMessageByTopic(topicId, messageId));
        }

        public static string LinkToMessage(MessageKey key)
        {
            Guard.Instance.ArgumentNotNull(()=>key, key);

            return LinkToMessage(key.TopicId, key.MessageId);
        }

        public static string LinkToMessage(Message message)
        {
            Guard.Instance.ArgumentNotNull(() => message, message)
                .ArgumentNotNull(() => message.Id, message.Id)
                .ArgumentNotNull(() => message.TopicId, message.TopicId);

            return LinkToMessage(message.TopicId, message.Id.Value);
        }
    }
}
