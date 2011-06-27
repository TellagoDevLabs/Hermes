using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Subscriptions;
using TellagoStudios.Hermes.RestService.Extensions;

namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SubscriptionResource : Resource
    {
        private readonly ICreateSubscriptionCommand createCommand;
        private readonly IUpdateSubscriptionCommand updateCommand;
        private readonly IDeleteSubscriptionCommand deleteCommand;
        private readonly IEntityById entityById;
        private readonly ISubscriptionsByGroup subscriptionsByGroup;
        private readonly ISubscriptionsByTopic subscriptionsByTopic;
        private readonly IGenericJsonPagedQuery genericJsonPagedQuery;

        public SubscriptionResource(
            ICreateSubscriptionCommand createCommand,
            IUpdateSubscriptionCommand updateCommand,
            IDeleteSubscriptionCommand deleteCommand,
            IEntityById entityById,
            ISubscriptionsByGroup subscriptionsByGroup,
            ISubscriptionsByTopic subscriptionsByTopic,
            IGenericJsonPagedQuery genericJsonPagedQuery)
        {
            this.createCommand = createCommand;
            this.updateCommand = updateCommand;
            this.deleteCommand = deleteCommand;
            this.entityById = entityById;
            this.subscriptionsByGroup = subscriptionsByGroup;
            this.subscriptionsByTopic = subscriptionsByTopic;
            this.genericJsonPagedQuery = genericJsonPagedQuery;
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage Create(Facade.SubscriptionPost subscription)
        {
            return ProcessPost(()  =>
                               {
                                   var instance = subscription.ToModel();
                                   createCommand.Execute(instance);
                                   return ResourceLocation.OfSubscription(instance.Id.Value);
                               });
        }

        [WebInvoke(UriTemplate = "", Method = "PUT")]
        public HttpResponseMessage Update(Facade.SubscriptionPut subscriptionPut)
        {
            return ProcessPut(() =>
            {
                var current = entityById.Get<Subscription>(subscriptionPut.Id.ToModel());

                current.Callback = subscriptionPut.Callback.ToModel();
                updateCommand.Execute(current);
            });
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Facade.Subscription> Get(Identity id)
        {
            return ProcessGet(() => entityById.Get<Subscription>(id).ToFacade());
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public HttpResponseMessage Delete(Identity id)
        {
            return ProcessDelete(() => deleteCommand.Execute(id));
        }

        [WebGet(UriTemplate = "topicgroup/{id}")] 
        public HttpResponseMessage<Facade.Subscription[]> GetByGroup(Identity id, HttpRequestMessage request)
        {
            return ProcessGet(() =>
            {
                var result = subscriptionsByGroup.Execute(id);
                return result
                    .Select(item => item.ToFacade())
                    .ToArray();
            });
        }

        [WebGet(UriTemplate = "topic/{id}")]
        public HttpResponseMessage<Facade.Subscription[]> GetByTopic(Identity id)
        {
            return ProcessGet(() =>
            {
                var result = subscriptionsByTopic.Execute(id);
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

            return ProcessGet(() =>
                               {
                                   var result = genericJsonPagedQuery.Execute<Subscription>(query, validatedSkip, validatedLimit);
                                   return result
                                       .Select(item => item.ToFacade())
                                       .ToArray();
                               });
                            
        }
    }
}