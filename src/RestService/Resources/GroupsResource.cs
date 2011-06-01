using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.RestService.Extensions;

namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class GroupsResource : Resource
    {
        private readonly IGroupService _groupService;

        public GroupsResource(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage<Facade.Group> Create(Facade.GroupPost topic)
        {
            return  Process(() =>
                               {
                                   var instance = topic.ToModel();
                                   var result = _groupService.Create(instance);
                                   return result.ToFacade();
                               });
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Facade.Group> Get(Identity id)
        {
            return Process(() => _groupService.Get(id).ToFacade());
        }

        [WebInvoke(UriTemplate = "", Method = "PUT")]
        public HttpResponseMessage<Facade.Group> Update(Facade.GroupPut group)
        {
            return Process(() =>
                               {
                                   var instance = group.ToModel();
                                   var result = _groupService.Update(instance);
                                   return result.ToFacade();
                               });
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public HttpResponseMessage Delete(Identity id)
        {
            return Process(()=>_groupService.Delete(id));
        }

        [WebGet(UriTemplate = "?query={query}&skip={skip}&limit={limit}")]
        public HttpResponseMessage<Facade.Group[]> GetAll(string query, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? new int?(skip) : new int?();
            var validatedLimit = limit > 0 ? new int?(limit) : new int?();

            return Process(() => Find(query, validatedSkip, validatedLimit));
        }

        #region Private members
        private Facade.Group[] Find(string query, int? skip, int? limit)
        {
            var result = _groupService.Find(query, skip, limit);
            return result.Select(item => item.ToFacade()).ToArray();
        }
        #endregion
    }
}