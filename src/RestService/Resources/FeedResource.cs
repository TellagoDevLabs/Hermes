using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class FeedResource
    {
        private readonly IGetWorkingFeedForTopic getWorkingFeedForTopic;
        private readonly IEntityById entityById;
        private readonly IMessageByMessageKey messageByMessageKey;

        public FeedResource(
            IGetWorkingFeedForTopic getWorkingFeedForTopic, 
            IEntityById entityById,
            IMessageByMessageKey messageByMessageKey)
        {
            this.getWorkingFeedForTopic = getWorkingFeedForTopic;
            this.entityById = entityById;
            this.messageByMessageKey = messageByMessageKey;
        }

        [WebGet(UriTemplate = "{topicId}")]
        public HttpResponseMessage Current(Identity topicId)
        {
            var workingFeed = getWorkingFeedForTopic.Execute(topicId);
            var syndicationFeed = CreateSyndicationFromFeed(topicId, workingFeed);
            return new HttpResponseMessage<SyndicationFeed>(syndicationFeed, HttpStatusCode.OK)
                       {
                           Content = {Headers = {ContentType = new MediaTypeHeaderValue("application/atom+xml")}}
                       };
        }

        [WebGet(UriTemplate = "{topicId}/history/{feedId}")]
        public HttpResponseMessage History(Identity topicId, Identity feedId)
        {
            var workingFeed = entityById.Get<Feed>(feedId);
            var syndicationFeed = CreateSyndicationFromFeed(topicId, workingFeed);
            return new HttpResponseMessage<SyndicationFeed>(syndicationFeed, HttpStatusCode.OK)
            {
                Content = { Headers = { ContentType = new MediaTypeHeaderValue("application/atom+xml") } }
            };
        }

        private SyndicationFeed CreateSyndicationFromFeed(Identity topicId, Feed currentFeed)
        {
            var topic = entityById.Get<Topic>(topicId);
            var feed = new SyndicationFeed(topic.Name, topic.Description, null)
                           {
                               Items = currentFeed.Entries.Select(e => MapEntryToSyndicationItem(topicId, e)).ToList(),
                               LastUpdatedTime = currentFeed.Updated,
                               Id = currentFeed.Id.ToString()
                           };

            feed.Links.Add(new SyndicationLink(ResourceLocation.OfCurrentTopicFeed(topicId))
                                {
                                    RelationshipType = "current",
                                    MediaType = "application/atom+xml"
                                });

            feed.Links.Add(new SyndicationLink(ResourceLocation.OfTopicFeed(topicId, currentFeed.Id.Value))
                               {
                                   RelationshipType = "self",
                                   MediaType = "application/atom+xml"
                               });

            if (currentFeed.PreviousFeed.HasValue)
            {
                feed.Links.Add(new SyndicationLink(ResourceLocation.OfTopicFeed(topicId, currentFeed.PreviousFeed.Value))
                                   {
                                       RelationshipType = "prev-archive",
                                       MediaType = "application/atom+xml"
                                   });
            }

            if (currentFeed.NextFeed.HasValue)
            {
                feed.Links.Add(new SyndicationLink(ResourceLocation.OfTopicFeed(topicId, currentFeed.NextFeed.Value))
                                   {
                                       RelationshipType = "next-archive",
                                       MediaType = "application/atom+xml"
                                   });
            }
            return feed;
        }


        private SyndicationItem MapEntryToSyndicationItem(Identity topicId, FeedEntry e)
        {
            var messageLink = ResourceLocation.OfMessageByTopic(topicId, e.MessageId);
            var message = messageByMessageKey.Get(new MessageKey {TopicId = topicId, MessageId = e.MessageId});

            var contentType = message.Headers.FirstOrDefault(h => h.Key == "Content-Type");
            var content = new UrlSyndicationContent(messageLink, contentType.Value.Aggregate((a,b)=>a +","+b));

            return new SyndicationItem(string.Format("Message {0}", e.MessageId),
                                       content,
                                       null,
                                       e.MessageId.ToString(), e.TimeStamp)
                       {
                           Links = { new SyndicationLink(messageLink) },
                           LastUpdatedTime = e.TimeStamp
                       };
        }
    }
   
}