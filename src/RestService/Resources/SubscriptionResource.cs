using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.RestService.Extensions;

namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SubscriptionResource : Resource
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionResource(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage<Facade.Subscription> Create(Facade.SubscriptionPost subscription)
        {
            return Process(() =>
                               {
                                   var instance = subscription.ToModel();
                                   var result = _subscriptionService.Create(instance);
                                   return result.ToFacade();
                               });
        }

        [WebInvoke(UriTemplate = "", Method = "PUT")]
        public HttpResponseMessage<Facade.Subscription> Update(Facade.SubscriptionPut subscriptionPut)
        {
            return Process(() =>
            {
                var instance = subscriptionPut.ToModel();
                var result = _subscriptionService.Update(instance);
                return result.ToFacade();
            });
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Facade.Subscription> Get(Identity id)
        {
            return Process(() => _subscriptionService.Get(id).ToFacade());
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public HttpResponseMessage Delete(Identity id)
        {
            return Process(() => _subscriptionService.Delete(id));
        }

        [WebGet(UriTemplate = "topicgroup/{id}")] 
        public HttpResponseMessage<Facade.Subscription[]> GetByGroup(Identity id, HttpRequestMessage request)
        {
            return Process(() =>
            {
                var result = _subscriptionService.GetByGroup(id);
                return result
                    .Select(item => item.ToFacade())
                    .ToArray();
            });
        }

        [WebGet(UriTemplate = "topic/{id}")]
        public HttpResponseMessage<Facade.Subscription[]> GetByTopic(Identity id)
        {
            return Process(() =>
            {
                var result = _subscriptionService.GetByTopic(id);
                return result
                    .Select(item => item.ToFacade())
                    .ToArray();
            });
        }

        [WebGet(UriTemplate = "?query={query}&skip={skip}&limit={limit}")]
        public HttpResponseMessage<Facade.Subscription[]> GetAll(string query, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return Process(() =>
                               {
                                   var result = _subscriptionService.Find(query, validatedSkip, validatedLimit);
                                   return result
                                       .Select(item => item.ToFacade())
                                       .ToArray();
                               });
                            
        }
    }
}