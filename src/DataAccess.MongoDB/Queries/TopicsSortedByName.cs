using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class TopicsSortedByName : MongoDbRepository, ITopicsSortedByName
    {
        public TopicsSortedByName(string connectionString) : base(connectionString)
        {
            
        }

        public IEnumerable<Topic> Execute()
        {
            var result = DB.GetCollectionByType<Topic>()
                           .FindAll()
                           .SetSortOrder(new SortByDocument("Name", 1));
            return result;
        }
    }
}