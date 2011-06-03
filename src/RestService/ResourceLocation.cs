using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService
{
    class ResourceLocation
    {
        public static string LinkToMessage(Message message)
        {
            Guard.Instance.ArgumentNotNull(() => message, message)
                .ArgumentNotNull(() => message.Id, message.Id)
                .ArgumentNotNull(() => message.TopicId, message.TopicId);

            return LinkToMessage(message.TopicId, message.Id.Value);
        }

        public static string LinkToMessage(Identity topicId, Identity messageId)
        {
            return string.Format("<link rel=\"{0}\" href=\"{1}\"/>", Constants.Relationships.Message, OfMessageByTopic(topicId, messageId));
        }

        public static string OfMessageByTopic(Identity topicId, Identity messageId)
        {
            return "/" + Constants.Routes.Messages + "/" + messageId + "/topic/" + topicId;
        }

    }
}
