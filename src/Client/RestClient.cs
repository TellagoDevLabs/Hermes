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

        public HttpWebResponse GetResponse(string operation, IEnumerable<Header> headers = null)
        {
            var url = CreateUrl(operation);
            return GetResponse(url, headers);
        }

        public HttpWebResponse GetResponse(Uri url, IEnumerable<Header> headers = null)
        {
            return CreateRequest(url, "GET", headers)
                .Send();
        }

        public T Get<T>(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            var url = CreateUrl(operation);
            return Get<T>(url, headers, webExceptionHandler);
        }

        public T Get<T>(Uri url, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            try
            {
                return CreateRequest(url, "GET", headers)
                    .Send(webExceptionHandler)
                    .Deserialize<T>();
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return default(T);
                }
                throw;
            }
        }

        public void Put<T>(string operation, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            Uri url = CreateUrl(operation);
            Put(url, data, headers, webExceptionHandler);
        }

        public void Put<T>(Uri url, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null) 
        {
            CreateRequest(url, "PUT", headers)
                .Serialize(data)
                .Send(webExceptionHandler);
        }

        public Uri Post<T>(string operation, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null, string contentType = null)
        {
            Uri url = CreateUrl(operation);
            return Post(url, data, headers, webExceptionHandler, contentType);
        }

        public Uri Post<T>(Uri url, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null, string contentType = null )
        {
            var httpWebRequest = CreateRequest(url, "POST", headers);
            if (!string.IsNullOrWhiteSpace(contentType)) httpWebRequest.ContentType = contentType;

            Uri location;
            using (var httpWebResponse = httpWebRequest
                .Serialize(data)
                .Send(webExceptionHandler))
            {
                location = httpWebResponse.GetLocation();
            }

            return location;
        }

        public void Delete(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            var url = CreateUrl(operation);
            Delete(url, headers, webExceptionHandler);
        }

        public void Delete(Uri url, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            CreateRequest(url, "DELETE", headers)
                .Send(webExceptionHandler);
        }

        public Stream GetStream(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            var url = CreateUrl(operation);
            return GetStream(url, headers, webExceptionHandler);
        }

        public Stream GetStream(Uri url, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            return CreateRequest(url, "GET", headers)
                .Send(webExceptionHandler)
                .GetResponseStream();
        }

        #region Private members

        private Uri CreateUrl(string operation)
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

            return url;
        }

        private HttpWebRequest CreateRequest(Uri url, string method, IEnumerable<Header> headers)
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

        #endregion
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

        public static HttpWebResponse Send(this HttpWebRequest request, Action<WebException> webExceptionHanlder = null)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (webExceptionHanlder!=null) webExceptionHanlder(e);
                else if (RestClient.WebExceptionHandler!=null) RestClient.WebExceptionHandler(e);
                else throw;
            }

            return null;
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
            if (response == null || response.StatusCode == HttpStatusCode.NotFound) return default(T);

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
            if (response==null) return null;

            var location = response.Headers[HttpResponseHeader.Location];
            return string.IsNullOrWhiteSpace(location) ? null : new Uri(location);
        }
    }
}
