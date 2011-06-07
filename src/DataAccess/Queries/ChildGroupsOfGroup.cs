using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class ChildGroupsOfGroup :  MongoDbRepository, IChildGroupsOfGroup
    {
        private MongoCollection<Group> groupCollection;

        public ChildGroupsOfGroup(string connectionString) 
             : base(connectionString)
        {
            groupCollection = DB.GetCollection<Group>(MongoDbConstants.Collections.Groups);
        }

        public bool HasChilds(Identity id)
        {
            return groupCollection.Exists(BuildQuery(id));
        }

        public IEnumerable<Group> GetChilds(Identity id)
        {
            return groupCollection.Find(BuildQuery(id));
        }

        public QueryDocument BuildQuery(Identity groupId)
        {
            string queryString = string.Format(@"{{""ParentId"" : {0}}}", groupId.ToBsonString());
            return queryString.ToQueryDocument();
        }
    }
}