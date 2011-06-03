using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Logging.Model;

namespace TellagoStudios.Hermes.Logging
{
    public class MongoDbLogRepository : MongoDbRepository, ILogRepository
    {
        private readonly MongoCollection<LogEntry> _logCollection;

        public MongoDbLogRepository(string connectionString)
            :base(connectionString)
        {
            if (!DB.CollectionExists(Constants.Collections.Log))
            {
                var optionsBuilder = new CollectionOptionsBuilder()
                    .SetAutoIndexId(true)
                    .SetCapped(true)
                    .SetMaxSize(1000000);
                
                var creationResult = DB.CreateCollection(Constants.Collections.Log, optionsBuilder);
                if (!creationResult.Ok)
                    throw new Exception(creationResult.ErrorMessage);
            }


            _logCollection = DB.GetCollection<LogEntry>(Constants.Collections.Log);
        }

        public LogEntry Create(LogEntry entry)
        {
            Guard.Instance.ArgumentNotNull(()=>entry, entry);

            _logCollection.Save(entry);

            return entry;
        }

        public LogEntry Get(Identity id)
        {
            var entry = MongoDbDriverExtensions.FindById<LogEntry>(_logCollection, id);
            return entry;
        }
        
        public void Truncate()
        {
            _logCollection.Drop();            
        }

        public IEnumerable<LogEntry> Find(string query, int? skip, int? limit)
        {
            Guard.Instance
                .ArgumentValid(()=>skip, () => (skip.HasValue && skip.Value < 0))
                .ArgumentValid(()=>limit, () => (limit.HasValue && limit.Value <= 0));

            var queryDoc = query.ToQueryDocument();
            var cursor = _logCollection.Find(queryDoc);

            if (skip.HasValue && skip.Value > 0) cursor = cursor.SetSkip(skip.Value);
            if (limit.HasValue && limit.Value >= 0) cursor = cursor.SetLimit(limit.Value);

            return cursor;
        }

    }
}