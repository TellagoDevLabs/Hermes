using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class MessageByMessageKey : MongoDbRepository, IMessageByMessageKey
    {
        public MessageByMessageKey(string connectionString)
            : base(connectionString)
        {}

        public bool Exist(MessageKey key)
        {
            string collectionName = MongoDbConstants.GetCollectionNameForMessage(key.TopicId);
            return DB.GetCollection<Message>(collectionName).Exists(key.MessageId);
        }

         public bool Exist(MessageKey key, string filter)
             var query = string.IsNullOrWhiteSpace(filter) ? 
             if (query.Contains(Constants.FieldNames.Id))

             string collectionName = MongoDbConstants.GetCollectionNameForMessage(key.TopicId);
             return DB.GetCollection<Message>(collectionName).Exists(query);
         }

        public Message Get(MessageKey key) 
        {
            string collectionName = MongoDbConstants.GetCollectionNameForMessage(key.TopicId);
            return DB.GetCollection<Message>(collectionName).FindById(key.MessageId);
        }
    }
}