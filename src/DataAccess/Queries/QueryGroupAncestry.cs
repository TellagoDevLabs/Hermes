using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.DataAccess.Queries
{
    public class QueryGroupAncestry : MongoDbRepository, IQueryGroupAncestors
    {
        private readonly MongoCollection<Group> groupCollection;

        public QueryGroupAncestry(string connectionString) : base(connectionString)
        {
            groupCollection = DB.GetCollection<Group>(MongoDbConstants.Collections.Groups);
        }

        public IEnumerable<Group> Execute(Group group)
        {
            return GetEnumeration(group).ToArray();
        }

        public IEnumerable<Group> Execute(Identity groupId)
        {
            return Execute(groupCollection.FindById(groupId));
        }

        private IEnumerable<Group> GetEnumeration(Group @group)
        {
            while (group != null)
            {
                yield return group;

                if(group.ParentId.HasValue)
                {
                    group = groupCollection.FindById(group.ParentId.Value);
                }
                else
                {
                    break;
                }
            }
        }
    }
}