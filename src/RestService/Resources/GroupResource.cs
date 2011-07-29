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
    public class GroupResource : Resource
    {
        private readonly IEntityById entityById;
        private readonly IUpdateGroupCommand updateGroupCommand;
        private readonly IDeleteGroupCommand deleteGroupCommand;
        private readonly ITopicsByGroup topicsByGroup;
        private readonly IGroupByName groupByName;

        public GroupResource(IEntityById entityById,
            IUpdateGroupCommand updateGroupCommand,
            IDeleteGroupCommand deleteGroupCommand,
            ITopicsByGroup topicsByGroup,
            IGroupByName groupByName)
        {
            this.entityById = entityById;
            this.updateGroupCommand = updateGroupCommand;
            this.deleteGroupCommand = deleteGroupCommand;
            this.topicsByGroup = topicsByGroup;
            this.groupByName = groupByName;
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

        [WebGet(UriTemplate = "{groupId}/topics?skip={skip}&limit={limit}")]
        public HttpResponseMessage<Facade.Topic[]> GetByGroup(Facade.Identity groupId, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return ProcessGet(() =>
                    topicsByGroup.GetTopics(groupId.ToModel(), validatedSkip, validatedLimit)
                        .Select(item => item.ToFacade())
                        .ToArray());
        }

        [WebGet(UriTemplate = "/?name={name}")]
        public HttpResponseMessage<Facade.Group> GetByName(string name)
        {
            return ProcessGet(() => groupByName.Get(name).ToFacade());
        }
    }
}