using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using TellagoStudios.Hermes.Client.Serialization;
using TellagoStudios.Hermes.Client.Util;

namespace TellagoStudios.Hermes.Client
{
    public class RestClient
    {
        private readonly Uri baseAddress;
        private readonly Uri proxy;

        public RestClient(Uri baseAddress, Uri proxy)
            : this(baseAddress)
        {
            this.proxy = proxy;
        }


        public RestClient(Uri baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        public static Action<WebException> WebExceptionHandler { get; set; }

        public HttpWebResponse GetResponse(string operation, 
                                            IEnumerable<Header> headers = null, 
                                            Action<WebException> webExceptionHandler = null)
        {
            return Client(operation, "GET", headers)
                .Send(webExceptionHandler);
        }

        public void Get(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            Client(operation, "GET", headers)
                .Send(webExceptionHandler);
        }

        public T Get<T>(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            var httpWebRequest = Client(operation, "GET", headers);
            using(var httpWebResponse = httpWebRequest.Send(webExceptionHandler))
            {
                var deserialized = httpWebResponse.Deserialize<T>();
                httpWebResponse.Close();
                return deserialized;
            }
        }

        public T GetFromUrl<T>(Uri url, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            return ExecuteRequest(url, "GET", headers)
                .Send(webExceptionHandler)
                .Deserialize<T>();
        }

        public void Put<T>(string operation, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null) 
        {
            Client(operation, "PUT", headers)
                .Serialize(data)
                .Send(webExceptionHandler);
        }

        public Uri Post<T>(string operation, T data, 
                           IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null,
                           string contentType = null )
        {
            var httpWebRequest = Client(operation, "POST", headers);
            if (contentType != null) httpWebRequest.ContentType = contentType;
            Uri location;

            using(var httpWebResponse = httpWebRequest.Serialize(data).Send(webExceptionHandler))
            {
                location = httpWebResponse.GetLocation();
            }

            return location;
        }

        public void Delete(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            Client(operation, "DELETE", headers)
                .Send(webExceptionHandler);
        }

        private HttpWebRequest Client(string operation, string method, IEnumerable<Header> headers)
        {
            Uri url;

            if (baseAddress == null && string.IsNullOrWhiteSpace(operation))
            {
                throw new InvalidOperationException(
                    "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
            }

            if (string.IsNullOrWhiteSpace(operation))
            {
                url = baseAddress;
            }
            else
            {
                //if (operation[0] != '/') operation = '/' + operation;

                var requestUri = new Uri(operation, UriKind.RelativeOrAbsolute);
                if (requestUri.IsAbsoluteUri)
                {
                    url = requestUri;
                }
                else if (baseAddress != null)
                {
                    url = new Uri(baseAddress, requestUri);
                }
                else
                {
                    throw new InvalidOperationException(
                        "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
                }
            }

            return ExecuteRequest(url, method, headers);
        }

        private HttpWebRequest ExecuteRequest(Uri url, string method, IEnumerable<Header> headers)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    HttpRequestHeader httpHeader;
                    if (Enum.TryParse(header.Name, true, out httpHeader))
                    {
                        request.SetHttpHeader(httpHeader, header.Value);
                    }
                    else
                    {
                        request.Headers.Add(header.Name, header.Value);
                    }
                }
            }
            if(proxy != null)
            {
                request.Proxy = new WebProxy(proxy, false);
            }
            return request;
        }

        public Stream GetStream(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            return Client(operation, "GET", headers)
                .Send(webExceptionHandler)
                .GetResponseStream();
        }

        public Stream GetStream(Uri url, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            return ExecuteRequest(url, "GET", headers)
                .Send(webExceptionHandler)
                .GetResponseStream();
        }
    }

    static class RestClientExtensions
    {
        public static void SetHttpHeader(this HttpWebRequest request, HttpRequestHeader header, string value)
        {
            switch (header)
            {
                case HttpRequestHeader.Accept:
                    request.Accept = value;
                    break;
                case HttpRequestHeader.Connection:
                    request.Connection = value;
                    break;
                case HttpRequestHeader.ContentLength:
                    request.ContentLength = long.Parse(value);
                    break;
                case HttpRequestHeader.ContentType:
                    request.ContentType = value;
                    break;
                case HttpRequestHeader.Date:
                    request.Date = DateTime.Parse(value);
                    break;
                case HttpRequestHeader.Expect:
                    request.Expect = value;
                    break;
                case HttpRequestHeader.Host:
                    request.Host = value;
                    break;
                case HttpRequestHeader.IfModifiedSince:
                    request.IfModifiedSince = DateTime.Parse(value);
                    break;
                case HttpRequestHeader.KeepAlive:
                    request.KeepAlive = bool.Parse(value);
                    break;
                case HttpRequestHeader.Referer:
                    request.Referer = value;
                    break;
                case HttpRequestHeader.TransferEncoding:
                    request.TransferEncoding = value;
                    break;
                case HttpRequestHeader.UserAgent:
                    request.UserAgent = value;
                    break;
                default:
                    request.Headers.Add(header.ToString(), value);
                    break;
            }
        }

        public static HttpWebResponse Send(this HttpWebRequest request, Action<WebException> webExceptionHandler)
        {
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (webExceptionHandler != null)
                {
                    webExceptionHandler(e);
                }
                else if (RestClient.WebExceptionHandler != null)
                {
                    RestClient.WebExceptionHandler(e);
                }
                else
                {
                    throw;
                }
            }

            return response;
        }
        
        static public HttpWebRequest Serialize<T>(this HttpWebRequest request, T data)
        {
            if (string.IsNullOrEmpty(request.ContentType))
            {
                request.ContentType = "application/xml"; 
            }
            var dataAsStream = data as Stream;
            if (dataAsStream != null)
            {
                using (var stream = request.GetRequestStream())
                {
                    dataAsStream.CopyTo(stream);
                    stream.Close();
                }
            }
            else
            {
                using (var stream = request.GetRequestStream())
                {
                    Serializer.Instance.Serialize(request.ContentType, stream, data);
                    stream.Flush();
                    stream.Close();
                }
            }
            return request;
        }

        static public T Deserialize<T>(this HttpWebResponse response)
        {
            if (response == null) return default(T);

            Debug.Assert(response.ContentType.Contains("xml"));

            using (var stream = response.GetResponseStream())
            {
                Debug.Assert(stream != null);

                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    var t = Activator.CreateInstance<T>();
                    var result = t as Stream;
                    stream.CopyTo(result);
                    if (stream.CanSeek)
                    {
                        result.Seek(0, SeekOrigin.Begin);
                    }
                    return t;
                }

                var entity = Serializer.Instance.Deserialize<T>(response.ContentType, stream);
                return entity;
            }
        }

        static public Uri GetLocation(this HttpWebResponse response)
        {
            var location = response.Headers[HttpResponseHeader.Location];
            return string.IsNullOrWhiteSpace(location) ? null : new Uri(location);
        }
    }
}
