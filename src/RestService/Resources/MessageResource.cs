using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using System.Net.Http;
using Identity = TellagoStudios.Hermes.Business.Model.Identity;
namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MessageResource : Resource
    {
        private readonly IMessageByMessageKey messageByMessageKey;

        public MessageResource(IMessageByMessageKey messageByMessageKey)
        {
            this.messageByMessageKey = messageByMessageKey;
        }

        [WebGet(UriTemplate = "{messageId}/topic/{topicId}")]
        public HttpResponseMessage Get(Identity topicId, Identity messageId, HttpRequestMessage request)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK, string.Empty);

            DoProcess(() => 
            {
                var key = new MessageKey { TopicId = topicId, MessageId = messageId }; 
                var message = messageByMessageKey.Get(key);
                if (message == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    PopulateHttpResponseMessage(ref response, message);
                }
            });

            return response;
        }

        #region Private methods

        private static void PopulateHttpResponseMessage(ref HttpResponseMessage response, Message message)
        {
            Guard.Instance.ArgumentNotNull(()=>message, message);

            response.Content = new ByteArrayContent(message.Payload ?? new byte[0]);
            var headerName = string.Empty;
            var headerValues = new string[0];
            try
            {
                foreach (var header in message.Headers)
                {
                    headerName = header.Key;
                    headerValues = header.Value;

                    if (Constants.HttpContentHeaders.Contains(header.Key, StringComparer.CurrentCultureIgnoreCase))
                    {
                        response.Content.Headers.Add(header.Key, header.Value);
                    }
                    else if (Constants.HttpResponseHeaders.Contains(header.Key, StringComparer.CurrentCultureIgnoreCase))
                    {
                        response.Headers.Add(header.Key, header.Value);
                    }
                    else if (Constants.HttpRequestHeaders.Contains(header.Key, StringComparer.CurrentCultureIgnoreCase))
                    {
                        // ignore header
                    }
                    else
                    {
                        // Custom header
                        response.Headers.Add(header.Key, header.Value);
                    }
                }
            }
            catch (Exception e)
            {
                var name = headerName ?? "The header's name is null.";
                var value = headerValues == null ? "The header's value is null." : headerValues.Aggregate((a,b)=> a + "; " + b);
                throw new ApplicationException(string.Format(Messages.InvalidHeader, name, value), e);
            }
        }

        #endregion
    }
}