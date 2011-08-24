using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using MongoDB.Driver.Builders;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class MessageKeysByTopic : MongoDbRepository, IMessageKeysByTopic
    {
        public MessageKeysByTopic(string connectionString)
            : base(connectionString)
        {}

        public IEnumerable<MessageKey> Get(Identity topicId, Identity? last  =null, int? skip = null, int? limit = null)
        {
            var col = DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(topicId));

            var query = last.HasValue ? Query.GT("_id", BsonValue.Create(last.Value)) : null;
            var cursor = col.Find(query);

            if (skip.HasValue) cursor.SetSkip(skip.Value);
            if (limit.HasValue) cursor.SetLimit(limit.Value);

            return cursor.Select(msg => new MessageKey
                {
                    TopicId = msg.TopicId,
                    MessageId = msg.Id.Value
                });
        }
    }
}