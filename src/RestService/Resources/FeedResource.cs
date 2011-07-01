using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
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

        public FeedResource(IGetWorkingFeedForTopic getWorkingFeedForTopic, IEntityById entityById)
        {
            this.getWorkingFeedForTopic = getWorkingFeedForTopic;
            this.entityById = entityById;
        }

        [WebGet(UriTemplate = "{topicId}")]
        public HttpResponseMessage Current(Identity topicId)
        {
            var workingFeed = getWorkingFeedForTopic.Execute(topicId);
            var syndicationFeed = CreateSyndicationFromFeed(topicId, workingFeed);
            return new HttpResponseMessage<SyndicationFeed>(syndicationFeed, HttpStatusCode.OK)
                       {
                           Content = { Headers = { ContentType = new MediaTypeHeaderValue("application/atom+xml") } }
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
                               Items = currentFeed.Entries.Select(e => MapEntryToSyndicationItem(topicId, e)).ToList()
                           };

            if (currentFeed.PreviousFeed.HasValue)
            {
                feed.Links.Add(new SyndicationLink(ResourceLocation.OfTopicFeed(topicId, currentFeed.PreviousFeed.Value))
                                   {
                                       RelationshipType = "prev"
                                   });
            }

            if (currentFeed.NextFeed.HasValue)
            {
                feed.Links.Add(new SyndicationLink(ResourceLocation.OfTopicFeed(topicId, currentFeed.NextFeed.Value))
                                   {
                                       RelationshipType = "next"
                                   });
            }
            return feed;
        }


        private static SyndicationItem MapEntryToSyndicationItem(Identity topicId, FeedEntry e)
        {
            return new SyndicationItem(string.Format("Message {0}", e.MessageId),
                                       new UrlSyndicationContent(ResourceLocation.OfMessageByTopic(topicId, e.MessageId), "application/xml"),
                                       null,
                                       e.MessageId.ToString(), e.TimeStamp);
        }
    }
   
}