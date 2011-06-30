using System;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Events;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Feeds
{
    public class AddMessageToFeedHandler : IHandler<NewMessageEvent>
    {
        private readonly IRepository<Feed> repository;
        private readonly IGetWorkingFeedForTopic query;

        public AddMessageToFeedHandler(IRepository<Feed> repository, IGetWorkingFeedForTopic query)
        {
            this.repository = repository;
            this.query = query;
        }

        public void Handle(NewMessageEvent @event)
        {

            var feed = query.Execute(@event.Message.TopicId);
            
            var entry = new FeedEntry
                            {
                                MessageId = @event.Message.Id.Value,
                                TimeStamp = DateTime.UtcNow
                            };

            feed.Updated = DateTime.UtcNow;
            feed.Entries.Insert(0, entry);

            repository.Update(feed);
        }
    }
}