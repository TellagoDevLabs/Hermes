using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Common;
using TellagoStudios.Hermes.DataAccess;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.Common.Model
{
    public static class MessageExtensions
    {
        public static BsonDocument ToMongoDocument(this Message message)
        {
            if (message == null) return null;

            var doc = new BsonDocument();
            if (!string.IsNullOrWhiteSpace(message.Id))
            {
                doc[MongoDbMessageRepository.FieldNames.Id] = new BsonObjectId(message.Id);
            }

            // System properties
            doc[MongoDbMessageRepository.FieldNames.ReceivedOn] = message.UtcReceivedOn;
            doc[MongoDbMessageRepository.FieldNames.TopicRef] =
                message.TopicId.ToDBRef(Constants.Relationships.Topic).ToBsonDocument();

            // Populate metadata
            doc[MongoDbMessageRepository.FieldNames.Payload] = message.Payload;
            doc[MongoDbMessageRepository.FieldNames.Headers] = message.Headers.StringValuesToMongoDocument();
            doc[MongoDbMessageRepository.FieldNames.PromotedProperties] = message.PromotedProperties.JsonValuesToMongoDocument();


            return doc;
        }

        public static BsonDocument JsonValuesToMongoDocument(this Dictionary<string, string> from)
        {
            if (from == null) return null;

            var doc = new BsonDocument();

            from.ForEach(item =>doc[item.Key] = item.Value.ToBsonValue());

            return doc;
        }

        public static BsonDocument StringValuesToMongoDocument(this Dictionary<string, string[]> from)
        {
            if (from == null) return null;

            var doc = new BsonDocument();

            from.ForEach(item =>
                             {
                                 if (item.Value == null)
                                 {
                                     doc[item.Key] = BsonNull.Value;
                                 }
                                 else if (item.Value.Length == 1)
                                 {
                                     doc[item.Key] = item.Value[0] ?? (BsonValue)BsonNull.Value;
                                 }
                                 else
                                 {
                                    doc[item.Key] = new BsonArray(item.Value);
                                 }
                             });

            return doc;
        }

        public static Message ToMessage(this BsonDocument doc)
        {
            if (doc == null) return null;

            var message = new Message
                              {
                                  Id = doc[MongoDbMessageRepository.FieldNames.Id].ToString(),
                                  Payload = doc[MongoDbMessageRepository.FieldNames.Payload].AsByteArray,
                                  UtcReceivedOn = doc[MongoDbMessageRepository.FieldNames.ReceivedOn].AsDateTime,
                                  TopicId = doc[MongoDbMessageRepository.FieldNames.TopicRef].AsBsonDocument["$id"].AsObjectId.ToString(),
                                  Headers = doc[MongoDbMessageRepository.FieldNames.Headers].AsBsonDocument.ToHeaders(),
                                  PromotedProperties = doc[MongoDbMessageRepository.FieldNames.PromotedProperties].AsBsonDocument.ToPromotedProperties()
                              };
            return message;
        }

        public static Dictionary<string, string> ToPromotedProperties(this BsonDocument doc)
        {
            var promotedProperties = new Dictionary<string, string>();

            if (doc != null)
            {
                doc.Elements.ForEach(element => promotedProperties.Add(element.Name, element.Value.ToJson()));
            }

            return promotedProperties;
        }

        public static Dictionary<string, string[]> ToHeaders(this BsonDocument doc)
        {
            var headers = new Dictionary<string, string[]>();

            if (doc != null)
            {
                doc.Elements.ForEach(element =>
                                         {
                                             if (element.Value.IsBsonNull)
                                             {
                                                 headers.Add(element.Name, null);
                                             }
                                             else if (element.Value.IsString)
                                             {
                                                 headers.Add(element.Name, new [] {element.Value.AsString});
                                             }
                                             else if (element.Value.IsBsonArray)
                                             {
                                                 var values = element.Value.AsBsonArray
                                                     .Select(v => v.AsString)
                                                     .ToArray();

                                                 headers.Add(element.Name, values);
                                             }
                                             else
                                             {
                                                 throw new InvalidOperationException(
                                                     string.Format(
                                                         Messages.InvalidHeaderValueType,
                                                             element.Value.BsonType.ToString()));
                                             }
                                         });
            }

            return headers;
        }
    }
}