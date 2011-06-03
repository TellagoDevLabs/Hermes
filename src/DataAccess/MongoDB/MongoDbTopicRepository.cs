using System;
using System.Diagnostics;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.DataAccess.MongoDB
{
    public class MongoDbTopicRepository : MongoDbRepository, ITopicRepository
    {
        private readonly MongoCollection<Topic> _topicsCollection;

        public MongoDbTopicRepository(string connectionString)
            :base(connectionString)
        {
            _topicsCollection = DB.GetCollection<Topic>(Constants.Collections.Topics);
        }

        public Topic Create(Topic topic)
        {
            Guard.Instance.ArgumentNotNull(()=>topic, topic);

            _topicsCollection.Save(topic);

            Debug.Assert(topic != null);
            Debug.Assert(topic.Id.HasValue);

            DB.CreateCollection(topic.MessagesCollectionName, new CollectionOptionsDocument());
            //TODO: Add indexies to message's collection

            return topic;
        }

        public bool ExistsById(Identity id)
        {
            return _topicsCollection.Exists(id);
        }

        public bool ExistsByQuery(string query)
        {
            Guard.Instance.ArgumentNotNullOrWhiteSpace(()=>query, query);

            var queryDoc = query.ToQueryDocument();

            return _topicsCollection.Exists(queryDoc);
        }

        public Topic Get(Identity id)
        {
            var result = _topicsCollection.FindById(id);
            return result;
        }

        public Topic Update(Topic topic)
        {
            Guard.Instance.ArgumentNotNull(()=>topic, topic);
            _topicsCollection.Save(topic);

            return topic;
        }

        public void Delete(Identity id)
        {
            _topicsCollection.Remove(id);

            var topic = new Topic { Id = id };
            DB.DropCollection(topic.MessagesCollectionName);
        }

        public IEnumerable<Topic> Find(string query, int? skip, int? limit)
        {
            Guard.Instance.ArgumentValid(()=>skip, () => (skip.HasValue && skip.Value < 0));
            Guard.Instance.ArgumentValid(()=>limit, () => (limit.HasValue && limit.Value <= 0));

            var queryDoc = query.ToQueryDocument();
            var cursor = _topicsCollection.Find(queryDoc);

            if (skip.HasValue && skip.Value > 0) cursor = cursor.SetSkip(skip.Value);
            if (limit.HasValue && limit.Value >= 0) cursor = cursor.SetLimit(limit.Value);

            return cursor;
        }

        public IEnumerable<Identity> GetTopicIdsInGroup(Identity groupId)
        {
            var queryString = QueryGetByGroup(groupId);
            var queryDoc = queryString.ToQueryDocument();
            var cursor = _topicsCollection.Find(queryDoc);

            return cursor.Select(t => t.Id.Value);
        }

        public string QueryGetByGroup(Identity groupId)
        {
            var query = "{\"GroupId\" : " + groupId.ToBsonString() + "}";
            return query;

        }

        public string QueryDuplicatedName(Topic topic)
        {
            return topic.Id.HasValue ?
                "{ \"Name\":\"" + topic.Name + "\", \"_id\" : { $ne : " + topic.Id.ToBsonString() + "} }" :
                "{ \"Name\":\"" + topic.Name + "\"}";
        }
    }
}