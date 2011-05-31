using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Repository;

namespace TellagoStudios.Hermes.DataAccess.MongoDB
{
    public class MongoDbGroupRepository : MongoDbRepository, IGroupRepository
    {
        private readonly MongoCollection<Group> _groupsCollection;

        public MongoDbGroupRepository(string connectionString)
            : base(connectionString)
        {
            _groupsCollection = DB.GetCollection<Group>(MongoDbConstants.Collections.Groups);
        }

        public Group Create(Group group)
        {
            Guard.Instance.ArgumentNotNull(()=>group, group);            

            //var doc = group.ToMongoDocument();
            _groupsCollection.Save(group);

            //var result = doc.ToGroup();
            return group;
        }

        public bool ExistsById(Identity id)
        {
            return _groupsCollection.Exists(id);
        }

        public bool ExistsByQuery(string query)
        {
            Guard.Instance.ArgumentNotNullOrWhiteSpace(()=>query, query);           

            var queryDoc = query.ToQueryDocument();

            return _groupsCollection.Exists(queryDoc);
        }

        public Group Get(Identity id)
        {
            var group = _groupsCollection.FindById(id);
            return group;
        }

        public Group Update(Group group)
        {
            Guard.Instance.ArgumentNotNull(()=>group, group);
            _groupsCollection.Save(group);
            return group;
        }

        public void Delete(Identity id)
        {
            _groupsCollection.Remove(id);
        }

        public IEnumerable<Group> Find(string query, int? skip, int? limit)
        {
            Guard.Instance.ArgumentValid(()=>skip, () => (skip.HasValue && skip.Value < 0));
            Guard.Instance.ArgumentValid(()=>limit, () => (limit.HasValue && limit.Value <= 0));

            var queryDoc = query.ToQueryDocument();
            var cursor = _groupsCollection.Find(queryDoc);

            if (skip.HasValue) cursor = cursor.SetSkip(skip.Value);
            if (limit.HasValue) cursor = cursor.SetLimit(limit.Value);

            return cursor;
        }

        public string QueryDuplicatedName(Group group)
        {
            return group.Id.HasValue ?
                "{ \"Name\":\"" + group.Name + "\", \"_id\" : { $ne : " + group.Id.ToBsonString() + "} }" :
                "{ \"Name\":\"" + group.Name + "\"}";
        }

        public string QueryGetChildGroups(Identity groupId)
        {
            return "{\"ParentId\" : " + groupId.ToBsonString() + "}";
        }
    }
}