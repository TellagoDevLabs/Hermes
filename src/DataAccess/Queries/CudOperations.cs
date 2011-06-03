using System;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class CudOperations<TDocument> : MongoDbRepository,
        ICudOperations<TDocument> where TDocument : DocumentBase
    {
        private readonly MongoCollection<TDocument> collection;

        public CudOperations(string connectionString) : base(connectionString)
        {
            collection = DB.GetCollection<TDocument>(MongoDbConstants.GetCollectionNameForType<TDocument>());
        }

        public void MakePersistent(TDocument document)
        {
            collection.Save(document);
        }

        public void MakeTransient(TDocument document)
        {
            if (document.Id != null) collection.Remove(document.Id.Value);
        }

        public void Update(TDocument document)
        {
            collection.Save(document);
        }
    }
}