using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Client
{
    public abstract class RestClient
    {
        protected static Uri BaseAddress { get; set; }
        public static Action<WebException> WebExceptionHandler { get; set; }

        protected HttpWebResponse GetResponse(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            return Client(operation, "GET", headers)
                .Send(webExceptionHandler);
        }

        protected void Get(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            Client(operation, "GET", headers)
                .Send(webExceptionHandler);
        }

        protected T Get<T>(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            return Client(operation, "GET", headers)
                .Send(webExceptionHandler)
                .Deserialize<T>();
        }

        protected void Put<T>(string operation, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null) 
        {
            Client(operation, "PUT", headers)
                .Serialize(data)
                .Send(webExceptionHandler);
        }

        protected U Put<T, U>(string operation, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            return Client(operation, "PUT", headers)
                .Serialize(data)
                .Send(webExceptionHandler)
                .Deserialize<U>();
        }

        protected void Post<T>(string operation, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null) 
        {
            Client(operation, "POST", headers)
                .Serialize(data)
                .Send(webExceptionHandler);
        }

        protected U Post<T, U>(string operation, T data, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            return Client(operation, "POST", headers)
                .Serialize(data)
                .Send(webExceptionHandler)
                .Deserialize<U>();
        }

        protected void Delete(string operation, IEnumerable<Header> headers = null, Action<WebException> webExceptionHandler = null)
        {
            Client(operation, "DELETE", headers)
                .Send(webExceptionHandler);
        }

        private static HttpWebRequest Client(string operation, string method, IEnumerable<Header> headers)
        {
            Uri url;

            if (BaseAddress == null && string.IsNullOrWhiteSpace(operation))
            {
                throw new InvalidOperationException(
                    "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
            }

            if (string.IsNullOrWhiteSpace(operation))
            {
                url = BaseAddress;
            }
            else
            {
                if (operation[0] != '/') operation = '/' + operation;

                var requestUri = new Uri(operation, UriKind.RelativeOrAbsolute);
                if (requestUri.IsAbsoluteUri)
                {
                    url = requestUri;
                }
                else if (BaseAddress != null)
                {
                    url = new Uri(BaseAddress, requestUri);
                }
                else
                {
                    throw new InvalidOperationException(
                        "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
                }
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            if (headers != null && headers.Any())
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
            return request;
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
                request.ContentType = "application/xml"; // TODO: Replace. We should support many content types
            }

            var stream = request.GetRequestStream();

            var dataAsStream = data as Stream;
            if (dataAsStream != null)
            {
                dataAsStream.CopyTo(stream);
            }
            else
            {
                var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
                var writer = XmlWriter.Create(stream, settings);

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade");

                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, data, ns);
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

                var reader = XmlReader.Create(stream);
                var serializer = new XmlSerializer(typeof(T));
                var entity = (T)serializer.Deserialize(reader);
                return entity;
            }
        }
    }
}
