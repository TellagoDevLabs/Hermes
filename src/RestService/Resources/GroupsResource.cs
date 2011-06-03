using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.RestService.Extensions;

namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class GroupsResource : Resource
    {
        private readonly IEntityById entityById;
        private readonly IGenericJsonPagedQuery genericJsonPagedQuery;
        private readonly ICreateGroupCommand createGroupCommand;
        private readonly IUpdateGroupCommand updateGroupCommand;
        private readonly IDeleteGroupCommand deleteGroupCommand;

        public GroupsResource(IEntityById entityById,
            IGenericJsonPagedQuery genericJsonPagedQuery,
            ICreateGroupCommand createGroupCommand, 
            IUpdateGroupCommand updateGroupCommand,
            IDeleteGroupCommand deleteGroupCommand)
        {
            this.entityById = entityById;
            this.genericJsonPagedQuery = genericJsonPagedQuery;
            this.createGroupCommand = createGroupCommand;
            this.updateGroupCommand = updateGroupCommand;
            this.deleteGroupCommand = deleteGroupCommand;
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage<Facade.Group> Create(Facade.GroupPost topic)
        {
            return  Process(() =>
                               {
                                   var instance = topic.ToModel();
                                   createGroupCommand.Execute(instance);
                                   return instance.ToFacade();
                               });
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Facade.Group> Get(Identity id)
        {
            return Process(() => entityById.Get<Group>(id).ToFacade());
        }

        [WebInvoke(UriTemplate = "", Method = "PUT")]
        public HttpResponseMessage<Facade.Group> Update(Facade.GroupPut group)
        {
            return Process(() =>
                               {
                                   var instance = group.ToModel();
                                   updateGroupCommand.Execute(instance);
                                   return instance.ToFacade();
                               });
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public HttpResponseMessage Delete(Identity id)
        {
            return Process(() => deleteGroupCommand.Execute(new Group { Id = id }));
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
            var result = genericJsonPagedQuery.Execute<Group>(query, skip, limit);
            return result.Select(item => item.ToFacade()).ToArray();
        }
        #endregion
    }
}