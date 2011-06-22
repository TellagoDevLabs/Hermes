using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
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

        public IEnumerable<Group> GetChildren(Identity id)
        {
            return groupCollection.Find(BuildQuery(id));
        }

        public IEnumerable<Identity> GetChildrenIds(Identity id)
        {
            var cursor =  groupCollection.Find(BuildQuery(id));
            cursor.SetFields("_id");
            return cursor.Select(g => g.Id.Value);
        }


        public IMongoQuery BuildQuery(Identity groupId)
        {
           return Query.EQ("ParentId", groupId.ToBson());
        }
    }
}