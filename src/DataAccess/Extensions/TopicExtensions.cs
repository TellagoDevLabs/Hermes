using System;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Common;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.Common.Model
{
    public static class TopicExtensions
    {
        public static BsonDocument ToMongoDocument(this Topic topic)
        {
            if (topic == null) return null;

            var doc = new BsonDocument();

            if (topic.Id!=null)
            {
                doc[MongoDbTopicRepository.FieldNames.Id] = BsonValue.Create(topic.Id.Value);
            }
            doc[MongoDbTopicRepository.FieldNames.Name] = topic.Name;
            doc[MongoDbTopicRepository.FieldNames.Description] = topic.Description;
            doc[MongoDbTopicRepository.FieldNames.GroupRef] = topic.Group.Id.Value.ToDBRef(Constants.Relationships.Group).ToBsonDocument();

            return doc;
        }
 
        public static Topic ToTopic(this BsonDocument doc) 
        {
            if (doc == null) return null;

            var topic = new Topic
                            {
                                Id = doc[MongoDbTopicRepository.FieldNames.Id].ToString(),
                                Name = doc[MongoDbTopicRepository.FieldNames.Name].AsString,
                                Description = doc[MongoDbTopicRepository.FieldNames.Description].AsString,
                                Group = new Group
                                {
                                    Id = doc[MongoDbTopicRepository.FieldNames.GroupRef].AsBsonDocument["$id"].AsObjectId.ToString()
                                }
                            };

            return topic;
        }
    }
}