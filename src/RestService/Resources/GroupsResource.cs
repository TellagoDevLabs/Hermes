using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.RestService.Extensions;

namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class GroupsResource : Resource
    {
        private readonly IGenericJsonPagedQuery genericJsonPagedQuery;
        private readonly ICreateGroupCommand createGroupCommand;

        public GroupsResource(
            IGenericJsonPagedQuery genericJsonPagedQuery,
            ICreateGroupCommand createGroupCommand)
        {
            this.genericJsonPagedQuery = genericJsonPagedQuery;
            this.createGroupCommand = createGroupCommand;
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage Create(Facade.GroupPost topic)
        {
            return  ProcessPost(() =>
                               {
                                   var instance = topic.ToModel();
                                   createGroupCommand.Execute(instance);
                                   return ResourceLocation.OfGroup(instance.Id.Value);
                               });
        }

        [WebGet(UriTemplate = "?query={query}&skip={skip}&limit={limit}")]
        public HttpResponseMessage<Facade.Group[]> GetAll(string query, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? new int?(skip) : new int?();
            var validatedLimit = limit > 0 ? new int?(limit) : new int?();

            return ProcessGet(() => Find(query, validatedSkip, validatedLimit));
        }

        private Facade.Group[] Find(string query, int? skip, int? limit)
        {
            var result = genericJsonPagedQuery.Execute<Group>(query, skip, limit);
            return result.Select(item => item.ToFacade()).ToArray();
        }
    }
}