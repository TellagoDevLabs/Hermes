using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.Facade;
using TellagoStudios.Hermes.RestService.Extensions;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;

namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class TopicsResource : Resource
    {
        private readonly ITopicService _topicService;

        public TopicsResource(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Topic> Get(Identity id)
        {
            return Process(() => _topicService.Get(id.ToModel()).ToFacade());
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage<Topic> Create(TopicPost topic)
        {
            return Process(() =>
                               {
                                   var instance = topic.ToModel();
                                   var result = _topicService.Create(instance);
                                   return result.ToFacade();
                               });
        }

        [WebInvoke(UriTemplate = "", Method = "PUT")]
        public HttpResponseMessage<Topic> Update(TopicPut topic)
        {
            return Process(() =>
                               {
                                   var instance = topic.ToModel();
                                   var result = _topicService.Update(instance);
                                   return result.ToFacade();
                               });
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public HttpResponseMessage Delete(Identity id)
        {
            return Process(() => _topicService.Delete(id.ToModel()));
        }

        [WebGet(UriTemplate = "?skip={skip}&limit={limit}&query={query}")]
        public HttpResponseMessage<Topic[]> GetAll(string query, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return Process(() => _topicService
                    .Find(query, validatedSkip, validatedLimit)
                    .Select(g => g.ToFacade())
                    .ToArray());
        }

        [WebGet(UriTemplate = "/group/{groupId}?skip={skip}&limit={limit}")]
        public HttpResponseMessage<Topic[]> GetByGroup(Identity groupId, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return Process(() => _topicService
                .GetByGroup(groupId.ToModel(), validatedSkip, validatedLimit)
                .Select(g => g.ToFacade())
                .ToArray());
        }
    }
}