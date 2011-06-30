using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Xml;
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
            var topic = entityById.Get<Topic>(topicId);
            var currentFeed = getWorkingFeedForTopic.Execute(topicId);
            
            var feed = new SyndicationFeed(topic.Name, topic.Description, new Uri("http://www.google.com"));
            feed.Items = currentFeed.Entries.Select(e =>
                                                    new SyndicationItem(string.Format("Message {0}", e.MessageId),
                                                                        new TextSyndicationContent("hello"),
                                                                        ResourceLocation.OfMessageByTopic(topicId,e.MessageId),
                                                                        e.MessageId.ToString(), e.TimeStamp)).ToList();

            var response = new HttpResponseMessage(HttpStatusCode.OK, "OK");
            response.Headers.Add("Content-Type", "application/atom+xml");

            var formatter = new Atom10FeedFormatter(feed);
            var memoryStream = new MemoryStream();
            formatter.WriteTo(XmlWriter.Create(memoryStream));
            response.Content = new StreamContent(memoryStream);
            return response;
        }
    }
   
}