using System;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Logging
{
    public abstract class MongoDbRepository
    {
        protected static MongoDatabase DB { get; private set; }
        protected static readonly object _sync = new object();

        public MongoDbRepository(string connectionString)
        {
            if (DB == null)
            {
                lock (_sync)
                {
                    if (DB == null)
                    {
                        Guard.Instance.ArgumentNotNullOrWhiteSpace(() => connectionString, connectionString);

                        BsonSerializer.RegisterSerializer(typeof(Identity), new IdentitySerializer());
                        BsonSerializer.RegisterIdGenerator(typeof(Identity?), new IdentityGenerator());
                     
                        var server = MongoServer.Create(connectionString);
                        var uri = new Uri(connectionString);
                        var dbName = uri.Segments[uri.Segments.Length - 1];

                        DB = server.GetDatabase(dbName);
                    }
                }
            }
        }
    }
}