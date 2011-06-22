using System;
using System.Linq;
using TellagoStudios.Hermes.Common;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace MongoDB.Driver
{
    public static class MongoDBDriverExtensions
    {
        public static T FindById<T>(this MongoCollection<T> collection, Guid id)
            where T : class
        {
            Guard.Instance.ArgumentNotNull(()=>collection, collection);
            
            try
            {
                var t = collection.FindOneById(id);
                return t;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Document not found
                return null;
            }
        }

        public static bool Exists<T>(this MongoCollection<T> collection, Guid id)
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

        //public static BsonDocument GetDBRef(this BsonDocument doc, MongoDatabase db)
        //{
        //    Guard.Instance
        //        .ArgumentValid(()=>doc, () => (doc == null || doc.IsBsonNull))
        //        .ArgumentNotNull(()=>db, db);

        //    var mongoRef = new MongoDBRef(doc["$ref"].AsString, doc["$id"].AsObjectId);
        //    var instance = db.FetchDBRef(mongoRef);
        //    return instance;
        //}

        public static void Remove<T>(this MongoCollection<T> collection, Guid id)
        {
            Guard.Instance.ArgumentNotNull(()=>collection, collection);            

            collection.Remove(new QueryDocument(Constants.FieldNames.Id, BsonValue.Create(id)));
        }

        //public static MongoDBRef ToDBRef(this Guid id, string href)
        //{
        //    Guard.Instance.ArgumentNotNullOrWhiteSpace(()=>href, href);

        //    return new MongoDBRef(href, BsonValue.Create(id));
        //}

        static public BsonDocument ToBsonDocument(this string from)
        {
            if (string.IsNullOrWhiteSpace(from)) return null;

            using (var reader = BsonReader.Create(from))
            {
                return BsonDocument.ReadFrom(reader);
            }
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