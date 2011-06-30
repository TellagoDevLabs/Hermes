using System;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using NUnit.Framework;
using TellagoStudios.Hermes.Client.Util;

namespace TellagoStudios.Hermes.Client.Tests.Util
{
    public class IntegrationTestBase
    {
        [SetUp] 
        public void TearDown()
        {
            var db = GetDatabase(ConfigurationManager.ConnectionStrings["db.connectionString"].ConnectionString);
            db.GetCollectionNames()
                .Select(db.GetCollection)
                .ToList().ForEach(c => c.RemoveAll());
        }

        public MongoDatabase GetDatabase(string connectionString)
        {
            Guard.Instance.ArgumentNotNullOrWhiteSpace(() => connectionString, connectionString);
            
            var server = MongoServer.Create(connectionString);
            var uri = new Uri(connectionString);
            var dbName = uri.Segments[uri.Segments.Length - 1];
            
            return  server.GetDatabase(dbName);
        }
    }
}