using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public static class Operations
    {
        public const string Topics = "topics";
        public const string Groups = "groups";
        public const string Messages = "messages";
        public const string Subscriptions = "subscriptions";

        static public string DeleteGroup(Identity id)
        {
            return Groups + "/" + id;
        }

        static public string GetTopicsByGroup(Identity id)
        {
            return Topics + "/group/" + id;
        }

        static public string GetMessagesBySubscription(Identity id)
        {
            return Messages + "/subscription/" + id;
        }

        static public string PostMessagesOnTopic(Identity id)
        {
            return Messages + "/topic/" + id;
        }

        static public string DeleteTopic(Identity id)
        {
            return Topics + "/" + id;
        }

        static public string DeleteSubscription(Identity id)
        {
            return Subscriptions + "/" + id;
        }

        internal static string GetMessageInTopic(Identity topicId, Identity messageId)
        {
            return Messages + "/" + messageId + "/topic/" + topicId;
        }
    }
}
