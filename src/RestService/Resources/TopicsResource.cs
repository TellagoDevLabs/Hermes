using System;
using System.Linq;
using System.Net;
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
    public class TopicsResource : Resource
    {
        private readonly IEntityById entityById;
        private readonly IGenericJsonPagedQuery genericJsonPagedQuery;
        private readonly ICreateTopicCommand createGroupCommand;
        private readonly IUpdateTopicCommand updateGroupCommand;
        private readonly IDeleteTopicCommand deleteGroupCommand;
        private readonly ITopicsByGroup topicsByGroup;

        public TopicsResource( IEntityById entityById,
            IGenericJsonPagedQuery genericJsonPagedQuery,
            ICreateTopicCommand createGroupCommand,
            IUpdateTopicCommand updateGroupCommand,
            IDeleteTopicCommand deleteGroupCommand,
            ITopicsByGroup topicsByGroup)
        {
            this.entityById = entityById;
            this.genericJsonPagedQuery = genericJsonPagedQuery;
            this.createGroupCommand = createGroupCommand;
            this.updateGroupCommand = updateGroupCommand;
            this.deleteGroupCommand = deleteGroupCommand;
            this.topicsByGroup = topicsByGroup;
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Topic> Get(Identity id)
        {
            return ProcessGet(() => entityById.Get<M.Topic>(id.ToModel()).ToFacade());
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage Create(TopicPost topic)
        {
            return ProcessPost(() =>
                               {
                                   var instance = topic.ToModel();
                                   createGroupCommand.Execute(instance);
                                   return ResourceLocation.OfTopic(instance.Id.Value);
                               });
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

        [WebGet(UriTemplate = "?skip={skip}&limit={limit}&query={query}")]
        public HttpResponseMessage<Topic[]> GetAll(string query, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return ProcessGet(() =>
                                   genericJsonPagedQuery.Execute<M.Topic>(query, validatedSkip, validatedLimit)
                                        .Select(i => i.ToFacade())
                                        .ToArray()
                               );
        }

        [WebGet(UriTemplate = "/group/{groupId}?skip={skip}&limit={limit}")]
        public HttpResponseMessage<Topic[]> GetByGroup(Identity groupId, int skip, int limit)
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