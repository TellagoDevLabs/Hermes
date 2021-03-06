﻿using System;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Resources
{
    public static class ResourceLocation
    {
        public static Uri BaseAddress {get; set; }

        private static Uri CreateUri(string resource)
        {
            Guard.Instance.ArgumentNotNull(() => BaseAddress, BaseAddress);
            return new Uri(BaseAddress, resource);            
        }
        
        public static Uri OfCreateMessageIngTopic(Identity topicId)
        {
            return CreateUri("/" + Constants.Routes.Messages + "/topic/" + topicId);
        }

        public static Uri OfTopics()
        {
            return CreateUri("/" + Constants.Routes.Topics  );
        }

        public static Uri OfTopicsByGroup(Identity id)
        {
            return CreateUri("/" + Constants.Routes.Group + "/" + id + "/topics");
        }

        public static Uri OfTopic(Identity id)
        {
            return CreateUri( "/" + Constants.Routes.Topic + "/" + id);
        }

        public static Uri OfTopicFeed(Identity topicId, Identity feedId)
        {
            return CreateUri(string.Format("/{0}/{1}/history/{2}", Constants.Routes.Feed, topicId, feedId));
        }

        public static Uri OfCurrentTopicFeed(Identity topicId)
        {
            return CreateUri(string.Format("/{0}/{1}", Constants.Routes.Feed, topicId));
        }

        public static Uri  OfGroup(Identity id)
        {
            return CreateUri( "/" + Constants.Routes.Group + "/" + id);
        }

        public static Uri OfSubscription(Identity id)
        {
            return CreateUri( "/" + Constants.Routes.Subscriptions + "/" + id);
        }

        public static Uri OfMessageByTopic(Message message)
        {
            Guard.Instance
                .ArgumentNotNull(() => message, message)
                .ArgumentNotNull(() => message.Id, message.Id);

            return OfMessageByTopic(message.TopicId, message.Id.Value);
        }

        public static Uri OfMessageByTopic(MessageKey key)
        {
            Guard.Instance.ArgumentNotNull(()=>key, key);

            return OfMessageByTopic(key.TopicId, key.MessageId);
        }

        public static Uri OfMessageByTopic(Identity topicId, Identity messageId)
        {
            return CreateUri( "/" + Constants.Routes.Message + "/" + messageId + "/topic/" + topicId);
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
