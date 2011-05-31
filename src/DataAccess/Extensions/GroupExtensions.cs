using System;

using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.Common.Model
{
    public static class GroupExtensions
    {
        public static BsonDocument ToMongoDocument(this Group group)
        {
            if (group == null) return null;

            var doc = new BsonDocument();

            if (group.Id!=null)
            {
                doc[MongoDbTopicRepository.FieldNames.Id] = BsonValue.Create(group.Id.Value);
            }
            doc[MongoDbGroupRepository.FieldNames.Name] = group.Name;
            doc[MongoDbGroupRepository.FieldNames.Description] = group.Description;

            if (group.ParentId!=null)
            {
                doc[MongoDbGroupRepository.FieldNames.ParentRef] = new MongoDBRef(MongoDbConstants.Collections.Groups, BsonValue.Create(group.ParentId)).ToBsonDocument();
            }

            return doc;
        }

        public static Group ToGroup(this BsonDocument doc) 
        {
            if (doc == null) return null;

            var topic = new Group
                            {
                                Name = doc[MongoDbTopicRepository.FieldNames.Name].AsString,
                                Description = doc[MongoDbTopicRepository.FieldNames.Description].AsString,
                                Id = doc[MongoDbTopicRepository.FieldNames.Id].ToString()
                            };


            if (doc.Contains(MongoDbGroupRepository.FieldNames.ParentRef))
            {
                topic.ParentId = doc[MongoDbGroupRepository.FieldNames.ParentRef].AsBsonDocument["$id"].AsObjectId.ToString();
            }

            return topic;
        }
    }
}