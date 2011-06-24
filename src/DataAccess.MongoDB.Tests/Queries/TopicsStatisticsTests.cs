using System;
using DataAccess.Tests.Repository;
using DataAccess.Tests.Util;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class TopicsStatisticsTests : MongoDbBaseFixture
    {
        private Topic fooTopic;
        private Topic barTopic;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var group = new Group {Name = "aaa"};
            mongoDb.GetCollectionByType<Group>().Insert(group);

            fooTopic = new Topic {Name = "FooTopic", Description = "a", GroupId = group.Id.Value };
            barTopic = new Topic {Name = "BarTopic", Description = "a", GroupId = group.Id.Value };

            mongoDb.GetCollectionByType<Topic>()
                    .InsertMany(fooTopic, barTopic);

            mongoDb.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(fooTopic.Id.Value))
                    .InsertMany(new Message
                                    {
                                        UtcReceivedOn = new DateTime(2011, 1, 1),
                                        TopicId = fooTopic.Id.Value
                                    },
                                    new Message
                                    {
                                        UtcReceivedOn = new DateTime(2011, 2, 2),
                                        TopicId = fooTopic.Id.Value
                                    },
                                    new Message
                                    {
                                        UtcReceivedOn = new DateTime(2011, 10, 2),
                                        TopicId = fooTopic.Id.Value
                                    });

            mongoDb.GetCollection<Message>(MongoDbConstants.GetCollectionNameForMessage(barTopic.Id.Value))
                .InsertMany(new Message
                                {
                                    UtcReceivedOn = new DateTime(2011, 1, 1),
                                    TopicId = barTopic.Id.Value
                                },
                            new Message
                                {
                                    UtcReceivedOn = new DateTime(2011, 2, 2),
                                    TopicId = barTopic.Id.Value
                                });
        }

        #endregion

        [Test]
        public void FromAllTimeShouldWork()
        {
            var query = new TopicsStatistics(base.connectionString);
            query.Execute();
        }
    }

    public class TopicsStatistics : MongoDbRepository, ITopicsStatistics
    {
        public TopicsStatistics(string connectionString) : base(connectionString)
        {
        }

        #region ITopicsStatistics Members

        public TopicStatisticsSingleResults Execute()
        {

            var topTopics = DB.GetCollectionByType<Topic>()
                .Group(null,
                       "Id",
                       new BsonDocument("count", "0"),
                       new BsonJavaScript("function(obj, prev) { prev.count = 1000 }"),
                       null);

            //db[\"this.msg_\" + obj.Id].count()
            //throw new NotImplementedException();

            //DB.GetCollectionByType<Message>()
            return null;
        }

        #endregion
    }
}