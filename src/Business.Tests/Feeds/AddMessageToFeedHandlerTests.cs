using System;
using System.Linq;
using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Events;
using TellagoStudios.Hermes.Business.Feeds;
using TellagoStudios.Hermes.Business.Model;

namespace Business.Tests.Feeds
{
    [TestFixture]
    public class AddMessageToFeedHandlerTests
    {
        [Test]
        public void WhenThereIsNotAchievedFeed_ThenCreateANewOneWithMessage()
        {
            
            var topicId = Identity.Random();
            var feed = new Feed { TopicId = topicId };
            var query = Mock.Of<IGetWorkingFeedForTopic>(q => q.Execute(topicId) == feed);

            var repository = new StubRepository<Feed>();
            
            var handler = CreateHandler(repository, query);
            var message = new Message  { Id = Identity.Random(), TopicId = topicId};

            handler.Handle(new NewMessageEvent { Message = message });

            repository.Updates
                .Satisfy(es => es.Count == 1
                            && es.First() == feed
                            && es.First().Entries.Any(e => e.MessageId == message.Id));
        }


        private static AddMessageToFeedHandler CreateHandler(
            IRepository<Feed> feedRepository = null, 
            IGetWorkingFeedForTopic query = null)
        {
            return new AddMessageToFeedHandler(
                        feedRepository ?? Mock.Of<IRepository<Feed>>(),
                        query);
        }
    }
}