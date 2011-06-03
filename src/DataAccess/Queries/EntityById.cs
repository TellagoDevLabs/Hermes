using System;
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

        public bool Exist<TCollection>(Identity id)
        {
            string collectionName = GetCollectionNameFor<TCollection>();

            return DB.GetCollection(collectionName).Exists(id);
        }

        //TODO don't like it. JRO
        private static string GetCollectionNameFor<TCollection>()
        {
            string collectionName;
            if(typeof(TCollection) == typeof(Group))
            {
                collectionName = MongoDbConstants.Collections.Groups;
            }else if (typeof(TCollection) == typeof(Subscription))
            {
                collectionName = MongoDbConstants.Collections.Subscriptions;
            }
            else if (typeof(TCollection) == typeof(Topic))
            {
                collectionName = MongoDbConstants.Collections.Topics;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknow collection {0}", typeof(TCollection).Name));
            }
            return collectionName;
        }

        public TDocument Get<TDocument>(Identity id) where TDocument : class
        {
            var collectionName = GetCollectionNameFor<TDocument>();
            return DB.GetCollection<TDocument>(collectionName).FindById(id);
        }
    }
}