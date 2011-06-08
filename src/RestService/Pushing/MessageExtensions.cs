using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Pushing
{
    public static class MessageExtensions
    {
        static public void PushToSubscription(this Message message, Subscription subscription)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, subscription.Callback.Url);

            switch (subscription.Callback.Kind)
            {
                case CallbackKind.Key:
                    request.Content = new StringContent(ResourceLocation.LinkToMessage(message), Encoding.ASCII, "application/xml");
                    break;
                case CallbackKind.Message:
                    request.PopulateWithMessage(message);
                    break;
                default:
                    throw new InvalidOperationException(string.Format(Business.Texts.CallbackKindUnknown, subscription.Callback.Kind));
            }

            new HttpClient().Send(request);
        }


        static private void PopulateWithMessage(this HttpRequestMessage request, Message message)
        {
            var req = request;
            Guard.Instance
                .ArgumentNotNull(() => req, request)
                .ArgumentNotNull(() => message, message);

            request.Content = new ByteArrayContent(message.Payload ?? new byte[0]);

            var headersToIgnore = Constants.HttpResponseHeaders.Union(Constants.HttpHeadersToIgnoreOnPush);

            var validHeaders = message.Headers.Where(h => !headersToIgnore.Contains(h.Key, StringComparer.CurrentCultureIgnoreCase));

            foreach (var header in validHeaders)
            {
                if (Constants.HttpContentHeaders.Contains(header.Key))
                {
                    request.Content.Headers.Add(header.Key, header.Value);
                }
                else
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}
