using System;
using System.Linq;
using DataAccess.Tests.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.MongoDB.Queries;

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
            feed.Satisfy(t => t.TopicId == topic.Id && t.Entries.Count == 0 && !t.PreviousFeed.HasValue);
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
            read.PreviousFeed.ToString().Should().Be.EqualTo(currentFeed.Id.ToString());

            mongoDb.GetCollectionByType<Feed>() 
                .FindById(currentFeed.Id.Value)
                .NextFeed.ToString().Should().Be.EqualTo(read.Id.ToString());
        }
         
    }
}