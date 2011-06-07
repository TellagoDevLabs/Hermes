using System;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class Repository<TDocument> : MongoDbRepository,
        IRepository<TDocument> where TDocument : EntityBase
    {
        private readonly MongoCollection<TDocument> collection;

        public Repository(string connectionString) : base(connectionString)
        {
            collection = DB.GetCollection<TDocument>(MongoDbConstants.GetCollectionNameForType<TDocument>());
        }

        public void MakePersistent(TDocument document)
        {
            collection.Save(document);
        }

        public void MakeTransient(Identity id)
        {
            collection.Remove(id);
        }

        public void Update(TDocument document)
        {
            collection.Save(document);
        }
    }
}