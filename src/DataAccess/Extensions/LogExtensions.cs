using System;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using MongoDB.Bson;

namespace TellagoStudios.Hermes.Common.Model
{
    public static class LogExtensions
    {
        public static BsonDocument ToMongoDocument(this LogEntry entry)
        {
            if (entry == null) return null;

            var doc = new BsonDocument();

            if (entry.Id!=null)
            {
                doc[MongoDbLogRepository.FieldNames.Id] = BsonValue.Create(entry.Id);
            }
            doc[MongoDbLogRepository.FieldNames.Message] = entry.Message;
            doc[MongoDbLogRepository.FieldNames.Ts] = entry.UtcTs;
            doc[MongoDbLogRepository.FieldNames.Type] = entry.Type.ToString();

            return doc;
        }
 
        public static LogEntry ToLogEntry(this BsonDocument doc) 
        {
            if (doc == null) return null;

            var entry = new LogEntry
                            {
                                Id = doc[MongoDbLogRepository.FieldNames.Id].AsGuid,
                                Message = doc[MongoDbLogRepository.FieldNames.Message].AsString,
                                UtcTs = doc[MongoDbLogRepository.FieldNames.Ts].AsDateTime,
                                Type = (LogEntryType)Enum.Parse(typeof(LogEntryType), doc[MongoDbLogRepository.FieldNames.Type].AsString),
                            };

            return entry;
        }
    }
}