using System.Linq;
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
        private readonly IGenericJsonPagedQuery genericJsonPagedQuery;
        private readonly ICreateTopicCommand createGroupCommand;
        

        public TopicsResource( 
            IGenericJsonPagedQuery genericJsonPagedQuery,
            ICreateTopicCommand createGroupCommand)
        {
            this.genericJsonPagedQuery = genericJsonPagedQuery;
            this.createGroupCommand = createGroupCommand;
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
    }
}