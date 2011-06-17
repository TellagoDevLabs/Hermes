using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class EntityById : MongoDbRepository, IEntityById
    {
        public EntityById(string connectionString) : base(connectionString)
        {}

        public bool Exist<TEntity>(Identity id)
        {
            string collectionName = MongoDbConstants.GetCollectionNameForType<TEntity>();
            return DB.GetCollection(collectionName).Exists(id);
        }

        public TEntity Get<TEntity>(Identity id) where TEntity : class
        {
            string collectionName = MongoDbConstants.GetCollectionNameForType<TEntity>();
            return DB.GetCollection<TEntity>(collectionName).FindById(id);
        }
    }
}