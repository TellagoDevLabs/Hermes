using System;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Commands
{
    public class Repository<TEntity> : MongoDbRepository,
        IRepository<TEntity> where TEntity : EntityBase
    {
        private readonly MongoCollection<TEntity> collection;

        public Repository(string connectionString) : base(connectionString)
        {
            collection = DB.GetCollection<TEntity>(MongoDbConstants.GetCollectionNameForType<TEntity>());
        }

        public void MakePersistent(TEntity entity)
        {
            collection.Save(entity);
        }

        public void MakeTransient(Identity id)
        {
            collection.Remove(id);
        }

        public void Update(TEntity entity)
        {
            collection.Save(entity);
        }
    }
}