using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Service;
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
        private readonly IMessageService messageService;
        
        public MessageResource (IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [WebInvoke(UriTemplate = "/topic/{id}", Method = "POST")]
        public HttpResponseMessage<Link> CreateMessageOnTopic(Identity id, HttpRequestMessage request)
        {
            return Process(()=>
            {
                var message = Create(request);
                message.TopicId = id;
                message = messageService.Create(message);
                return message.ToLink();
            });
        }

        [WebInvoke(UriTemplate = "/topicgroup/{id}", Method = "POST")]
        public HttpResponseMessage<Link[]> CreateMessageOnGroup(Identity id, HttpRequestMessage request)
        {
            return Process(() =>
            {
                var message = Create(request);
                var messages = messageService.CreateByGroup(id, message);
                return messages
                    .Select(m => m.ToLink())
                    .ToArray();
            });
        }

        [WebGet(UriTemplate = "/subscription/{id}")]
        public HttpResponseMessage<Link[]> GetBySubscription(Identity id)
        {
            return Process(() => 
                messageService.GetMessageKeysBySubscription(id)
                    .Select(key => key.ToLink()).ToArray()
            );
        }

        [WebGet(UriTemplate = "{messageId}/topic/{topicId}")]
        public HttpResponseMessage Get(Identity topicId, Identity messageId, HttpRequestMessage request)
        {
            return Process((response) => 
            {
                var key = new MessageKey { TopicId = topicId, MessageId = messageId }; 
                var message = messageService.Get(key);
                if (message == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    PopulateHttpResponseMessage(ref response, message);
                }
            });
        }

        [WebGet(UriTemplate = "topic/{topicId}")]
        public HttpResponseMessage<Link[]> GetForTopic(Identity topicId)
        {
            return Process(() =>
            {
                var messageKeys = messageService.GetForTopic(topicId);
                return messageKeys
                    .Select(messageKey => messageKey.ToLink())
                    .ToArray();
            });
        }

        [WebGet(UriTemplate = "topicgroup/{groupId}")]
        public HttpResponseMessage<Link[]> GetForGroup(Identity groupId)
        {
            return Process(() =>
            {
                var messageKeys = messageService.GetForGroup(groupId);
                return messageKeys
                    .Select(messageKey => messageKey.ToLink())
                    .ToArray();
            });
        }

        #region Private methods

        private Message Create(HttpRequestMessage request)
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

        private void PopulateHttpResponseMessage(ref HttpResponseMessage response, Message message)
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