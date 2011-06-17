using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class MessageRepository : MongoDbRepository, IMessageRepository
    {

        public MessageRepository(string connectionString)
            : base(connectionString)
        {
        }

        public void MakePersistent(Message entity)
        {
            var collection = DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(entity.TopicId));
            collection.Save(entity);
        }

        public void MakeTransient(MessageKey key)
        {
                        var collection = DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(key.TopicId));
                        collection.Remove(key.MessageId);
        }

        public void Update(Message entity)
        {
            var collection = DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(entity.TopicId));
            collection.Save(entity);
        }
    }
}