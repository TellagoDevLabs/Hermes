using System;
using System.Linq;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Repository;

namespace TellagoStudios.Hermes.DataAccess.MongoDB
{
    public class MongoDbRetryRepository :MongoDbRepository, IRetryRepository
    {
        private readonly MongoCollection<Retry> _retriesCollection;

        public MongoDbRetryRepository(string connectionString)
            :base(connectionString)
        {
            _retriesCollection = DB.GetCollection<Retry>(Constants.Collections.Retries);            
        }

        public Retry Create(Retry retry)
        {
            Guard.Instance.ArgumentNotNull(()=>retry, retry);

            _retriesCollection.Save(retry);

            return retry;
        }

        public Retry Get(Identity id)
        {
            var result = _retriesCollection.FindById(id);
            return result;
        }

        public Retry Update(Retry retry)
        {
            Guard.Instance.ArgumentNotNull(()=>retry, retry);
            _retriesCollection.Save(retry);

            return retry;
        }

        public void Delete(Identity id)
        {
            _retriesCollection.Remove(id);
        }

        public IEnumerable<Retry> Find(string query, int? skip, int? limit)
        {
            Guard.Instance.ArgumentValid(()=>skip, () => (skip.HasValue && skip.Value < 0));
            Guard.Instance.ArgumentValid(()=>limit, () => (limit.HasValue && limit.Value <= 0));

            var queryDoc = query.ToQueryDocument();
            var cursor = _retriesCollection.Find(queryDoc);

            if (skip.HasValue && skip.Value > 0) cursor = cursor.SetSkip(skip.Value);
            if (limit.HasValue && limit.Value >= 0) cursor = cursor.SetLimit(limit.Value);

            return cursor;
        }
    }
}