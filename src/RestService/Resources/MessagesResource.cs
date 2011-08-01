using System;
using System.Diagnostics;
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
    public class MessagesResource : Resource
    {
        private readonly ICreateMessageCommand createMessageCommand;
        private readonly IMessageKeysByTopic messageKeysByTopic;
        private readonly IMessageKeysByGroup messageKeysByGroup;
        private readonly IMessageKeysBySubscription messageKeysBySubscription;


        public MessagesResource(
            ICreateMessageCommand createMessageCommand,
            IMessageKeysByTopic messageKeysByTopic,
            IMessageKeysByGroup messageKeysByGroup,
            IMessageKeysBySubscription messageKeysBySubscription)
        {
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

        [WebGet(UriTemplate = "/subscription/{id}?last={last}&skip={skip}&limit={limit}")]
        public HttpResponseMessage<Link[]> GetBySubscription(Identity id, Identity last, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();
            var validatedLast = last != Identity.Empty ? last : new Identity?();

            return ProcessGet(() => messageKeysBySubscription
                    .Get(id, validatedLast, validatedSkip, validatedLimit)
                    .Select(key => key.ToLink())
                    .ToArray()
            );
        }

        [WebGet(UriTemplate = "topic/{id}?last={last}&skip={skip}&limit={limit}")]
        public HttpResponseMessage<Link[]> GetForTopic(Identity id, Identity last, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit : new int?();
            var validatedLast = last != Identity.Empty ? last : new Identity?();

            return ProcessGet(() => messageKeysByTopic
                    .Get(id, validatedLast, validatedSkip, validatedLimit)
                    .Select(key => key.ToLink())
                    .ToArray()
            );
        }

        [WebGet(UriTemplate = "topicgroup/{id}?last={last}&skip={skip}&limit={limit}")]
        public HttpResponseMessage<Link[]> GetForGroup(Identity id, Identity last, int skip, int limit)
        {
            // set valid values of opional parameters
            var validatedSkip = skip > 0 ? skip : new int?();
            var validatedLimit = limit > 0 ? limit  : new int?();
            var validatedLast = last != Identity.Empty ? last : new Identity?();

            return ProcessGet(() => messageKeysByGroup
                    .Get(id, validatedLast, validatedSkip, validatedLimit)
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