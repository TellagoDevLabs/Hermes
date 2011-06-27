using MongoDB.Driver;

namespace DataAccess.Tests.Util
{
    internal static class MongoExtensions
    {
        public static MongoCollection<T> InsertMany<T>(this MongoCollection<T> collection, params T[] documents)
        {
            collection.InsertBatch(documents);
            return collection;
        }
    }
}