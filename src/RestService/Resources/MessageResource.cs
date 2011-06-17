using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Messages;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Extensions;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.ApplicationServer.Http;
using Identity = TellagoStudios.Hermes.Business.Model.Identity;
using Link = TellagoStudios.Hermes.Facade.Link;
namespace TellagoStudios.Hermes.RestService.Resources
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MessageResource : Resource
    {
        private readonly IMessageByMessageKey messageByMessageKey;
        private readonly ICreateMessageCommand createMessageCommand;
        private readonly IMessageKeysByTopic messageKeysByTopic;
        private readonly IMessageKeysByGroup messageKeysByGroup;
        private readonly IMessageKeysBySubscription messageKeysBySubscription;


        public MessageResource(IMessageByMessageKey messageByMessageKey, 
            ICreateMessageCommand createMessageCommand,
            IMessageKeysByTopic messageKeysByTopic,
            IMessageKeysByGroup messageKeysByGroup,
            IMessageKeysBySubscription messageKeysBySubscription)
        {
            this.messageByMessageKey = messageByMessageKey;
            this.createMessageCommand = createMessageCommand;
            this.messageKeysBySubscription = messageKeysBySubscription;
            this.messageKeysByTopic = messageKeysByTopic;
            this.messageKeysByGroup= messageKeysByGroup;
        }

        [WebInvoke(UriTemplate = "/topic/{id}", Method = "POST")]
        public HttpResponseMessage CreateMessageOnTopic(Identity id, HttpRequestMessage request)
        {
            return ProcessPost(()=>
            {
                var message = Create(request);
                message.TopicId = id;
                createMessageCommand.Execute(message);
                return ResourceLocation.OfMessageByTopic(message);
            });
        }

        [WebGet(UriTemplate = "/subscription/{id}?skip={skip}&limit={limit}")]
        public HttpResponseMessage<Link[]> GetBySubscription(Identity id, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return ProcessGet(() => messageKeysBySubscription
                    .Get(id, validatedSkip, validatedLimit)
                    .Select(key => key.ToLink())
                    .ToArray()
            );
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

        [WebGet(UriTemplate = "topic/{id}?skip={skip}&limit={limit}")]
        public HttpResponseMessage<Link[]> GetForTopic(Identity id, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();

            return ProcessGet(() => messageKeysByTopic
                    .Get(id, validatedSkip, validatedLimit)
                    .Select(key => key.ToLink())
                    .ToArray()
            );
        }

        [WebGet(UriTemplate = "topicgroup/{id}?skip={skip}&limit={limit}")]
        public HttpResponseMessage<Link[]> GetForGroup(Identity id, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit  : new int?();

            return ProcessGet(() => messageKeysByGroup
                    .Get(id, validatedSkip, validatedLimit)
                    .Select(key => key.ToLink())
                    .ToArray()
            );
        }

        #region Private methods

        private static Message Create(HttpRequestMessage request)
        {
            var message = new Message
                              {
                                  // Populate system properties
                                  UtcReceivedOn = DateTime.UtcNow,

                                  // Populate post's body
                                  Payload = request.Content.ReadAsByteArray()
                              };

            var index = Constants.PrivateHeaders.PromotedProperty.Length;
            request.Headers
                .Where(h => h.Key.StartsWith(Constants.PrivateHeaders.PromotedProperty))
                .ForEach(h => { message.PromotedProperties[h.Key.Substring(index)] = h.Value.FirstOrDefault(); } );

            IEnumerable<string> values;
            if (request.Headers.TryGetValues(Constants.PrivateHeaders.PromotedProperties, out values))
            {
                foreach (var value in values)
                {
                    foreach (var item in value.Split(','))
                    {
                        var prop = item.Split('=');
                        message.PromotedProperties[prop[0]] = prop[1];
                    }
                }
            }

            // Populate headers
            request.Headers
                .Where(h => !h.Key.StartsWith(Constants.PrivateHeaders.Prefix))
                .Union(request.Content.Headers)
                .ForEach(h => message.Headers.Add(h.Key, h.Value.ToArray()));

            // Invokes repository
            return message;

        }

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