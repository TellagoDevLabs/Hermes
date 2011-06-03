using System.Collections.Generic;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class GenericJsonPagedQuery :  MongoDbRepository, IGenericJsonPagedQuery
    {
        public GenericJsonPagedQuery(string connectionString) 
            : base(connectionString)
        {}

        public IEnumerable<T> Execute<T>(string query, int? skip, int? limit)
        {
            Guard.Instance.ArgumentValid(() => skip, () => (skip.HasValue && skip.Value < 0));
            Guard.Instance.ArgumentValid(() => limit, () => (limit.HasValue && limit.Value <= 0));
            var collection = DB.GetCollection<T>(MongoDbConstants.GetCollectionNameForType<T>());
            var queryDoc = query.ToQueryDocument();
            var cursor = collection.Find(queryDoc);
            if (skip.HasValue) cursor = cursor.SetSkip(skip.Value);
            if (limit.HasValue) cursor = cursor.SetLimit(limit.Value);
            return cursor;
        }
    }
}