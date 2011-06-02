using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using TellagoStudios.Hermes;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess;

namespace MongoDB.Driver
{
    public static class MongoDbDriverExtensions
    {
        public static T FindById<T>(this MongoCollection<T> collection, Identity id)
            where T : class
        {
            Guard.Instance.ArgumentNotNull(()=>collection, collection);
            
            try
            {
                var t = collection.FindOneById(id.ToBson());
                return t;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Document not found
                return null;
            }
        }

        public static bool Exists<T>(this MongoCollection<T> collection, Identity id)
        {
            Guard.Instance.ArgumentNotNull(()=>collection, collection);

            return Exists(collection, new QueryDocument(Constants.FieldNames.Id, BsonValue.Create(id)));
        }

        public static bool Exists<T>(this MongoCollection<T> collection, IMongoQuery query)
        {
            Guard.Instance
                .ArgumentNotNull(()=>collection, collection)
                .ArgumentNotNull(()=>query, query);

            try
            {
                var cursor = collection.Find(query);
                cursor.SetLimit(1);
                return cursor.Any();
            }
            catch (ArgumentOutOfRangeException)
            {
                // Document not found
                return false;
            }
        }

        public static void Remove<T>(this MongoCollection<T> collection, Identity id)
        {
            Guard.Instance.ArgumentNotNull(()=>collection, collection);            

            collection.Remove(new QueryDocument(Constants.FieldNames.Id, BsonValue.Create(id)));
        }

        static public QueryDocument ToQueryDocument(this string from)
        {
            if (string.IsNullOrWhiteSpace(from)) return null;

            using (var reader = BsonReader.Create(from))
            {
                var doc = BsonDocument.ReadFrom(reader);
                var query = new QueryDocument(doc.Elements);
                return query;
            }
        }
    }
}