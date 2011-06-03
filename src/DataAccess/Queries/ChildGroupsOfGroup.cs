using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class ChildGroupsOfGroup :  MongoDB.MongoDbRepository, IChildGroupsOfGroup
    {
        private MongoCollection<Group> groupCollection;

        public ChildGroupsOfGroup(string connectionString) 
             : base(connectionString)
        {
            groupCollection = DB.GetCollection<Group>(MongoDbConstants.Collections.Groups);
        }

        public bool HasChilds(Group group)
        {
            return groupCollection.Exists(BuildQuery(group.Id.Value));
        }

        public IEnumerable<Group> GetChilds(Group group)
        {
            return groupCollection.Find(BuildQuery(group.Id.Value));
        }

        public QueryDocument BuildQuery(Identity groupId)
        {
            string queryString = string.Format(@"{{""ParentId"" : {0}}}", groupId.ToBsonString());
            return queryString.ToQueryDocument();
        }
    }
}