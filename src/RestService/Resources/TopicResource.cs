using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using TellagoStudios.Hermes.Business.Topics;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Facade;
using TellagoStudios.Hermes.RestService.Extensions;
using M= TellagoStudios.Hermes.Business.Model;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;

namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class TopicResource : Resource
    {
        private readonly IEntityById entityById;
        private readonly IUpdateTopicCommand updateGroupCommand;
        private readonly IDeleteTopicCommand deleteGroupCommand;
        private readonly ITopicByName topicByName;
        

        public TopicResource( IEntityById entityById,
            IUpdateTopicCommand updateGroupCommand,
            IDeleteTopicCommand deleteGroupCommand,
            ITopicByName topicByName)
        {
            this.entityById = entityById;
            this.updateGroupCommand = updateGroupCommand;
            this.deleteGroupCommand = deleteGroupCommand;
            this.topicByName = topicByName;
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Topic> Get(Identity id)
        {
            return ProcessGet(() => entityById.Get<M.Topic>(id.ToModel()).ToFacade());
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public HttpResponseMessage Update(Identity id, TopicPut topic)
        {
            //todo id?mmm
            return ProcessPut(() =>
                               {
                                   var instance = topic.ToModel();
                                   updateGroupCommand.Execute(instance);
                               });
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public HttpResponseMessage Delete(Identity id)
        {
            return ProcessDelete(() => deleteGroupCommand.Execute(id.ToModel()));
        }

        [WebGet(UriTemplate = "/?name={name}&groupId={groupId}")]
        public HttpResponseMessage<Topic> GetByName(string name, string groupId)
        {
            var validatedGroupId = string.IsNullOrWhiteSpace(groupId) ? new M.Identity?() : new M.Identity(groupId);
            return ProcessGet(() => topicByName.Get(name, validatedGroupId).ToFacade());
        }
    }
}