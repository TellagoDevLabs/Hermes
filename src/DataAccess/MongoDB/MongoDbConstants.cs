using System;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.DataAccess.MongoDB
{
    public static class MongoDbConstants
    {
        public const string DBName = "hermes";

        public static class Collections
        {
            public const string Subscriptions = "subscriptions";
            public const string Topics = "topics";
            public const string Groups = "groups";
        }

        //TODO don't like it. JRO
        public static string GetCollectionNameForType<TDocumentType>()
        {
            string collectionName;
            if (typeof(TDocumentType) == typeof(Group))
            {
                collectionName = Collections.Groups;
            }
            else if (typeof(TDocumentType) == typeof(Subscription))
            {
                collectionName = Collections.Subscriptions;
            }
            else if (typeof(TDocumentType) == typeof(Topic))
            {
                collectionName = Collections.Topics;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknow collection {0}", typeof(TDocumentType).Name));
            }
            return collectionName;
        }
    }
}