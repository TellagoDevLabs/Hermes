using System;
using System.Linq;
using DataAccess.Tests.Repository;
using DataAccess.Tests.Util;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

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
            var result = query.Execute();
            result.MostActiveAllTime
                .Satisfy(mostActives => mostActives.Any(ma => ma.Name == "FooTopic" && ma.MessageCount == 3)
                                     && mostActives.Any(ma => ma.Name == "BarTopic" && ma.MessageCount == 2));
        }
    }
}