using System;
using TellagoStudios.Hermes.Common;
using MongoDB.Bson;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.Common.Model
{
    public static class RetryExtensions
    {
        public static BsonDocument ToMongoDocument(this Retry retry)
        {
            if (retry == null) return null;

            Guard.Instance
                .ArgumentNotNull(()=>retry.Message, retry.Message)
                .ArgumentNotNull(()=>retry.Subscription, retry.Subscription);

            var doc = new BsonDocument();

            if (!String.IsNullOrEmpty(retry.Id))
            {
                doc[MongoDbRetryRepository.FieldNames.Id] = new BsonObjectId(retry.Id);
            }

            doc[MongoDbRetryRepository.FieldNames.Message] = retry.Message.ToMongoDocument();
            doc[MongoDbRetryRepository.FieldNames.Subscription] = retry.Subscription.ToMongoDocument();
            doc[MongoDbRetryRepository.FieldNames.LastTry] = retry.UtcLastTry;
            doc[MongoDbRetryRepository.FieldNames.Count] = retry.Count;

            return doc;
        }

        public static Retry ToRetry(this BsonDocument doc)
        {
            if (doc == null) return null;

            var retry = new Retry
                            {
                                Id = doc[MongoDbRetryRepository.FieldNames.Id].ToString(),
                                Message = doc[MongoDbRetryRepository.FieldNames.Message].AsBsonDocument.ToMessage(),
                                Subscription = doc[MongoDbRetryRepository.FieldNames.Subscription].AsBsonDocument.ToSubscription(),
                                UtcLastTry = doc[MongoDbRetryRepository.FieldNames.LastTry].AsDateTime,
                                Count = doc[MongoDbRetryRepository.FieldNames.Count].AsInt32
                            };

            return retry;
        }
    }
}