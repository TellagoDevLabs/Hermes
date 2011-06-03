using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class EntityById : MongoDbRepository, IEntityById
    {
        public EntityById(string connectionString) : base(connectionString)
        {}

        public bool Exist<TDocument>(Identity id)
        {
            string collectionName = MongoDbConstants.GetCollectionNameForType<TDocument>();
            return DB.GetCollection(collectionName).Exists(id);
        }

        public TDocument Get<TDocument>(Identity id) where TDocument : class
        {
            string collectionName = MongoDbConstants.GetCollectionNameForType<TDocument>();
            return DB.GetCollection<TDocument>(collectionName).FindById(id);
        }
    }
}