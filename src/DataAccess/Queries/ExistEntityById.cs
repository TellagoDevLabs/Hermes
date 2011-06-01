using System;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class ExistEntityById : MongoDbRepository, IExistEntityById
    {
        public ExistEntityById(string connectionString) : base(connectionString)
        {}

        public bool Execute<TCollection>(Identity id)
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

            return DB.GetCollection(collectionName).Exists(id);
        }
    }
}