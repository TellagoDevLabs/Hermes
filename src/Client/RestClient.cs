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

        protected HttpWebResponse GetResponse(string operation, IEnumerable<Header> headers = null)
        {
            return Client(operation, "GET", headers)
                .Send();
        }

        protected void Get(string operation, IEnumerable<Header> headers = null)
        {
            Client(operation, "GET", headers)
                .Send();
        }

        protected T Get<T>(string operation, IEnumerable<Header> headers = null)
        {
            return Client(operation, "GET", headers)
                .Send()
                .Deserialize<T>();
        }
        
        protected void Put<T>(string operation, T data, IEnumerable<Header> headers = null) 
        {
            Client(operation, "PUT", headers)
                .Serialize(data)
                .Send();
        }

        protected U Put<T, U>(string operation, T data, IEnumerable<Header> headers = null)
        {
            return Client(operation, "PUT", headers)
                .Serialize(data)
                .Send()
                .Deserialize<U>();
        }

        protected void Post<T>(string operation, T data, IEnumerable<Header> headers = null) 
        {
            Client(operation, "POST", headers)
                .Serialize(data)
                .Send();
        }

        protected U Post<T, U>(string operation, T data, IEnumerable<Header> headers = null)
        {
            return Client(operation, "POST", headers)
                .Serialize(data)
                .Send()
                .Deserialize<U>();
        }

        protected void Delete(string operation, IEnumerable<Header> headers = null)
        {
            Client(operation, "DELETE", headers)
                .Send();
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
                    request.Headers.Add(header.Name, header.Value);
                }
            }
            return request;
        }
    }

    static class RestClientExtensions
    {
        public static HttpWebResponse Send(this HttpWebRequest request)
        {
            var response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        static public HttpWebRequest Serialize<T>(this HttpWebRequest request, T data)
        {
            request.ContentType = "application/xml";  // TODO: Replace. We should support many content types
            var stream = request.GetRequestStream();

            var dataAsStream = data as Stream;
            if (dataAsStream != null)
            {
                dataAsStream.CopyTo(stream);
            }
            else
            {
                var settings = new XmlWriterSettings {OmitXmlDeclaration = true};
                var writer = XmlWriter.Create(stream, settings);

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade");

                var serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(writer, data, ns);
            }
            return request;
        }

        static public T Deserialize<T>(this HttpWebResponse response)
        {
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
