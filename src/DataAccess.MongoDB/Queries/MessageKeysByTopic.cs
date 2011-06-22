using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class MessageKeysByTopic : MongoDbRepository, IMessageKeysByTopic
    {
        public MessageKeysByTopic(string connectionString)
            : base(connectionString)
        {}

        public IEnumerable<MessageKey> Get(Identity topicId, int? skip = null, int? limit = null)
        {
            var col = DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(topicId));
            var cursor = col.FindAll();

            if (skip.HasValue) cursor.SetSkip(skip.Value);
            if (limit.HasValue) cursor.SetSkip(limit.Value);

            return cursor.Select(msg => new MessageKey
                {
                    TopicId = msg.TopicId,
                    MessageId = msg.Id.Value
                });
        }
    }
}