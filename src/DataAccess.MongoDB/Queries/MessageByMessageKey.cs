using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
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