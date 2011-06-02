using System;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public abstract class CudOperations<T> : MongoDbRepository , ICudOperations<T> where T : DocumentBase
    {
        private readonly MongoCollection<T> collection;

        protected CudOperations(string connectionString) : base(connectionString)
        {
            collection = DB.GetCollection<T>(CollectionName);
        }

        protected abstract string CollectionName { get; }

        public void MakePersistent(T document)
        {
            collection.Save(document);
        }

        public void MakeTransient(T document)
        {
            if (document.Id != null) collection.Remove(document.Id.Value);
        }

        public void Update(T document)
        {
            collection.Save(document);
        }
    }

    public class CUDGroups : CudOperations<Group>
    {
        public CUDGroups(string connectionString) : base(connectionString)
        {}

        protected override string CollectionName
        {
            get { return MongoDbConstants.Collections.Groups; }
        }
    }
}