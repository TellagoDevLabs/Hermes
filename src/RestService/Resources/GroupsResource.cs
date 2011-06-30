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
using Topic = TellagoStudios.Hermes.Facade.Topic;

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
        private readonly ITopicsByGroup topicsByGroup;

        public GroupsResource(IEntityById entityById,
            IGenericJsonPagedQuery genericJsonPagedQuery,
            ICreateGroupCommand createGroupCommand, 
            IUpdateGroupCommand updateGroupCommand,
            IDeleteGroupCommand deleteGroupCommand,
            ITopicsByGroup topicsByGroup)
        {
            this.entityById = entityById;
            this.genericJsonPagedQuery = genericJsonPagedQuery;
            this.createGroupCommand = createGroupCommand;
            this.updateGroupCommand = updateGroupCommand;
            this.deleteGroupCommand = deleteGroupCommand;
            this.topicsByGroup = topicsByGroup;
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

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Facade.Group> Get(Identity id)
        {
            return ProcessGet(() => entityById.Get<Group>(id).ToFacade());
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public HttpResponseMessage Update(Identity id, Facade.GroupPut group)
        {
            return ProcessPut(() =>
                               {
                                   var instance = group.ToModel();
                                   updateGroupCommand.Execute(instance);
                               });
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public HttpResponseMessage Delete(Identity id)
        {
            return ProcessDelete(() => deleteGroupCommand.Execute(id));
        }

        [WebGet(UriTemplate = "?query={query}&skip={skip}&limit={limit}")]
        public HttpResponseMessage<Facade.Group[]> GetAll(string query, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? new int?(skip) : new int?();
            var validatedLimit = limit > 0 ? new int?(limit) : new int?();

            return ProcessGet(() => Find(query, validatedSkip, validatedLimit));
        }

        #region Private members
        private Facade.Group[] Find(string query, int? skip, int? limit)
        {
            var result = genericJsonPagedQuery.Execute<Group>(query, skip, limit);
            return result.Select(item => item.ToFacade()).ToArray();
        }
        #endregion

        [WebGet(UriTemplate = "{groupId}/topics?skip={skip}&limit={limit}")]
        public HttpResponseMessage<Topic[]> GetByGroup(Facade.Identity groupId, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return ProcessGet(() =>
                    topicsByGroup.GetTopics(groupId.ToModel(), validatedSkip, validatedLimit)
                        .Select(item => item.ToFacade())
                        .ToArray());
        }
    }
}