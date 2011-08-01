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
        private readonly IUpdateSubscriptionCommand updateCommand;
        private readonly IDeleteSubscriptionCommand deleteCommand;
        private readonly IEntityById entityById;

        public SubscriptionResource(
            IUpdateSubscriptionCommand updateCommand,
            IDeleteSubscriptionCommand deleteCommand,
            IEntityById entityById)
        {
            this.updateCommand = updateCommand;
            this.deleteCommand = deleteCommand;
            this.entityById = entityById;
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
    }
}