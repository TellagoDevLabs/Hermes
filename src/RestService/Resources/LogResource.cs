using System;
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
    public class LogResource : Resource
    {
        private readonly ILogService _logService;

        public LogResource(ILogService logService)
        {
            _logService = logService;
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Facade.LogEntry> Get(Identity id)
        {
            return Process(() => _logService.Get(id).ToFacade());
        }
        

        [WebInvoke(UriTemplate = "?", Method = "DELETE")]
        public HttpResponseMessage Truncate(string id)
        {
            return Process(() => _logService.Truncate());
        }

        [WebGet(UriTemplate = "?skip={skip}&limit={limit}&query={query}")]
        public HttpResponseMessage<Facade.LogEntry[]> GetAll(string query, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return Process(() => Find(query, validatedSkip, validatedLimit));
        }


        #region Private members

        private Facade.LogEntry[] Find(string query, int? skip, int? limit)
        {
            if (skip.HasValue && skip.Value < 0) throw new ArgumentOutOfRangeException("skip");
            if (limit.HasValue && limit.Value <= 0) throw new ArgumentOutOfRangeException("limit");

            var result = _logService.Find(query, skip, limit);

            return result.Select(item => item.ToFacade()).ToArray();
        }

        #endregion
    }
}