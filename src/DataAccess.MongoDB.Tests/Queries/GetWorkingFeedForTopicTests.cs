using System;
using System.Linq;
using DataAccess.Tests.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace DataAccess.Tests.Queries
{
    [TestFixture]
    public class GetWorkingFeedForTopicTests : Repository.MongoDbBaseFixture
    {
        private Topic topic;

        [SetUp]
        public void SetUp()
        {
            var group = new Group{Name = "TheSuperGroup"};
            mongoDb.GetCollectionByType<Group>()
                   .Insert(group);

            topic = new Topic
                            {
                                GroupId = group.Id.Value
                            };

            mongoDb.GetCollectionByType<Topic>()
                   .Insert(topic);
        }

        [Test]
        public void WhenThereIsNoFeed_ThenCreateSaveAndReturnANewOne()
        {
            var getWorkingFeedForTopic = new GetWorkingFeedForTopic(connectionString);
            var feed = getWorkingFeedForTopic.Execute(topic.Id.Value);
            feed.Satisfy(t => t.TopicId == topic.Id && t.Entries.Count == 0);
        }


        [Test]
        public void WhenCurrentFeedIsNotFull_ThenReturnThatFeed()
        {
            var getWorkingFeedForTopic = new GetWorkingFeedForTopic(connectionString);
            var oldFeed = new Feed
            {
                Updated = DateTime.UtcNow.AddMinutes(-2),
                TopicId = topic.Id.Value
            };
            var currentFeed = new Feed
                           {
                               Updated = DateTime.UtcNow.AddMinutes(-1),
                               TopicId = topic.Id.Value
                           };
            mongoDb.GetCollectionByType<Feed>()
                .InsertMany(oldFeed, currentFeed);

            var read = getWorkingFeedForTopic.Execute(topic.Id.Value);

            read.Should().Be.EqualTo(currentFeed);
        }

        [Test]
        public void WhenCurrentFeedIsFull_ThenReturnANewFeed()
        {
            var getWorkingFeedForTopic = new GetWorkingFeedForTopic(connectionString);
            var oldFeed = new Feed
            {
                Updated = DateTime.UtcNow.AddMinutes(-2),
                TopicId = topic.Id.Value
            };
            var currentFeed = new Feed
            {
                Updated = DateTime.UtcNow.AddMinutes(-1),
                TopicId = topic.Id.Value,
                Entries = Enumerable.Range(0, 10)
                                    .Select(i => new FeedEntry{MessageId = Identity.Random(12)})
                                    .ToList()
            };
            mongoDb.GetCollectionByType<Feed>()
                .InsertMany(oldFeed, currentFeed);

            var read = getWorkingFeedForTopic.Execute(topic.Id.Value);

            read.Should().Not.Be.EqualTo(currentFeed);
        }
         
    }

    public class GetWorkingFeedForTopic : MongoDbRepository, IGetWorkingFeedForTopic
    {
        private readonly MongoCollection<Feed> feeds;
        private object lck = new object();

        public GetWorkingFeedForTopic(string connectionString) : base(connectionString)
        {
            feeds = DB.GetCollectionByType<Feed>();

        }

        public Feed Execute(Identity topicId)
        {
            Feed result;
            lock (lck)
            {
                result = feeds.Find(Query.EQ("TopicId", topicId.ToBson()))
                     .SetSortOrder(SortBy.Descending("Updated"))
                     .SetLimit(1).FirstOrDefault();

                if (result == null || result.Entries.Count >= 10)
                {
                    result = new Feed
                    {
                        TopicId = topicId
                    };

                    feeds.Insert(result);
                }    
            }
            return result;
        }
    }
}