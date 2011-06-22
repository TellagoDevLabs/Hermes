using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class MessageKeysByGroup : MongoDbRepository, IMessageKeysByGroup
    {
        private IChildGroupsOfGroup childGroupsOfGroup;
        private ITopicsByGroup topicByGroup;

        public MessageKeysByGroup(string connectionString, IChildGroupsOfGroup childGroupsOfGroup, ITopicsByGroup topicByGroup)
            : base(connectionString)
        {
            this.childGroupsOfGroup = childGroupsOfGroup;
            this.topicByGroup = topicByGroup;
        }

        public IEnumerable<MessageKey> Get(Identity groupId, int? skip = null, int? limit = null)
        {
            var topicIds = GetGroupTopicIdsAndDescendants(groupId);
            var keys = GetMessageKeys(topicIds);

            if (skip.HasValue) keys = keys.Skip(skip.Value);
            if (limit.HasValue) keys = keys.Take(limit.Value);

            return keys;
        }

        private IEnumerable<Identity> GetGroupTopicIdsAndDescendants(Identity groupId)
        {
            var topics = topicByGroup.GetTopicIds(groupId);
            foreach (var childTopicId in topics)
            {
                yield return childTopicId;
            }

            var children = childGroupsOfGroup.GetChildrenIds(groupId);
            foreach (var child in children)
            {
                var grandChildren  = GetGroupTopicIdsAndDescendants(child);
                foreach (var grandChild in grandChildren)
                {
                    yield return grandChild;
                }
            }
        }

        private IEnumerable<MessageKey> GetMessageKeys(IEnumerable<Identity> topicIds)
        {
            foreach (var topicId in topicIds)
            {
                var col = DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(topicId));
                var cursor = col.FindAll()
                    .SetFields("_id");

                foreach (var message in cursor)
                {
                    yield return new MessageKey {MessageId = message.Id.Value, TopicId = topicId};
                }
            }
        }
    }
}