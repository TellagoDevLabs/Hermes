using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TellagoStudios.Hermes.DataAccess.MongoDB
{
    public class MongoDbMessageRepository : MongoDbRepository, IMessageRepository
    {
        public MongoDbMessageRepository(string connectionString)
            :base(connectionString)
        {
        }

        public Message Create(Message message)
        {
            Guard.Instance.ArgumentNotNull(()=>message, message);

            var msgCollection = GetCollection(message.TopicId);
            msgCollection.Save(message);

            return message;
        }

        public IEnumerable<MessageKey> GetMessageKeys(Identity topicId, string query)
        {
            var msgCollection = GetCollection(topicId);
            var queryDoc = query.ToQueryDocument();

            var cursor = string.IsNullOrWhiteSpace(query) ?
                    msgCollection.FindAll() :
                    msgCollection.Find(queryDoc);
            
            cursor.SetFields(Constants.FieldNames.Id, Constants.FieldNames.TopicId);
            return cursor.Select(msg => new MessageKey
                                           {
                                               TopicId = msg.TopicId,
                                               MessageId = msg.Id.Value
                                           });
        }

        public Message Get(MessageKey key)
        {
            Guard.Instance.ArgumentNotNull(()=>key, key);

            var msgCollection = GetCollection(key.TopicId);
            var msg = msgCollection.FindById(key.MessageId);
            return msg;
        }

        public bool Exists(MessageKey key, string filter)
        {
            Guard.Instance.ArgumentNotNull(() => key, key);

            var query = string.IsNullOrWhiteSpace(filter) ? 
                new QueryDocument() :
                filter.ToQueryDocument();

            if (query.Contains(Constants.FieldNames.Id))
            {
                if (query[Constants.FieldNames.Id] != key.MessageId.ToBson())
                {
                    // Filter already has a different message's id. So the query will always return false.
                    return false;
                }
            }
            else
            {
                // The filter does not include the message's id. Then add it to que query.
                query.Add(Constants.FieldNames.Id, key.MessageId.ToBson());
            }

            // Run query on appropriate collection
            var msgCollection = GetCollection(key.TopicId);
            return msgCollection.Exists(query);
        }

        private MongoCollection<Message> GetCollection(Identity topicId)
        {
            var topic = new Topic { Id = topicId };
            return DB.GetCollection<Message>(topic.MessagesCollectionName, SafeMode.True);
        }
    }
}