using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Commands
{
    public class MessageRepository : MongoDbRepository, IMessageRepository
    {
        public MessageRepository(string connectionString)
            : base(connectionString)
        {
        }

        #region IMessageRepository Members

        public void MakePersistent(Message entity)
        {
            MongoCollection<Message> collection =
                DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(entity.TopicId));
            collection.Save(entity);
        }

        public void MakeTransient(MessageKey key)
        {
            MongoCollection<Message> collection =
                DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(key.TopicId));
            collection.Remove(key.MessageId);
        }

        public void Update(Message entity)
        {
            MongoCollection<Message> collection =
                DB.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(entity.TopicId));
            collection.Save(entity);
        }

        #endregion
    }
}