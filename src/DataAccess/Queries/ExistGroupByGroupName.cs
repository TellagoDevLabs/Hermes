using System;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class ExistGroupByGroupName : MongoDbRepository, IExistGroupByGroupName
    {
        public ExistGroupByGroupName(string connectionString) 
            : base(connectionString)
        {}

        public bool Execute(string groupName)
        {
            var qd = new QueryDocument("Name", groupName);
            return DB.GetCollection(MongoDbConstants.Collections.Groups).Exists(qd);
        }
    }
}