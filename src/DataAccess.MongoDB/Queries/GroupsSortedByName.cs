using System.Collections.Generic;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class GroupsSortedByName : MongoDbRepository, IGroupsSortedByName 
    {
        public GroupsSortedByName(string connectionString) 
            : base(connectionString)
        {}

        public IEnumerable<Group> Execute()
        {
            var result = DB.GetCollectionByType<Group>()
                           .FindAll()
                           .SetSortOrder(new SortByDocument("Name", 1));
            return result;
        }
    }
}