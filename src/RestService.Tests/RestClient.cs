using System;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using TellagoStudios.Hermes.RestService.Facade;

namespace RestService.Tests
{
    public class RestClient
    {
        public Uri Url { get; protected set; }

        public RestClient(Uri serviceUrl)
        {
            if (serviceUrl == null)
                throw new ArgumentNullException("serviceUrl");

            Url = serviceUrl;
        }

        public T ExecuteGet<T>(string operation, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            using (var client = CreateClient(this.Url))
            {
                var uri = Combine(operation);
                using (HttpResponseMessage response = client.Get(uri))
                {
                    Assert.AreEqual(expectedCode, response.StatusCode);
                    
                    return response.Content.DeserializeToEntity<T>();
                }
            }
        }

        public void ExecutePut<T>(string operation, T entity, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            DoPut(operation, entity, null);
        }

        public U ExecutePut<T, U>(string operation, T entity, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            U instance = default(U);

            DoPut(operation, entity, content => instance = content.DeserializeToEntity<U>(), expectedCode);

            return instance;
        }

        public void ExecutePost<T>(string operation, T entity, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            DoPost<T>(operation, entity, null);
        }

        public U ExecutePost<T, U>(string operation, T entity, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            U instance = default(U);

            DoPost(operation, entity, content => instance = content.DeserializeToEntity<U>(), expectedCode);

            return instance;
        }

        public void ExecuteDelete(string operation, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            using (var client = CreateClient(this.Url))
            {
                using (HttpResponseMessage response = client.Delete(Combine(operation)))
                {
                    Assert.AreEqual(expectedCode, response.StatusCode);
                }
            }
        }

        #region Private methods

        private HttpClient CreateClient(Uri url)
        {
            HttpClient client = new HttpClient(url);
            //client.TransportSettings.Credentials = CredentialCache.DefaultCredentials;

            return client;
        }

        private void DoPost<T>(string operation, T entity, Action<HttpContent> action, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            using (var client = new HttpClient(Url))
            {
                var content = entity.SerializeToContent();
                using (HttpResponseMessage response = client.Post(Combine(operation), content))
                {
                    Assert.AreEqual(expectedCode, response.StatusCode);
                    if (action != null)
                    {
                        action(response.Content);
                    }
                }
            }
        }

        private void DoPut<T>(string operation, T entity, Action<HttpContent> action, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            using (var client = new HttpClient(Url))
            {
                var content = entity.SerializeToContent();
                using (HttpResponseMessage response = client.Put(Combine(operation), content))
                {
                    Assert.AreEqual(expectedCode, response.StatusCode);
                    if (action != null)
                    {
                        action(response.Content);
                    }
                }
            }
        }

        private Uri Combine(string operation)
        {
            var uri = new Uri(Url + "/" + operation);
            return uri;
        }
        #endregion
    }

    static class RestClientExtensions
    {
        static public HttpContent SerializeToContent<T>(this T from)
        {
            HttpContent content;  
            if (from == null)
            {
                content = new StringContent("", Encoding.ASCII, "application/xml");
            }

            else if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                content = new StreamContent(from as Stream);
            }

            else
            {
                using (var stream = new MemoryStream())
                {
                    var settings = new XmlWriterSettings {OmitXmlDeclaration = true};
                    var writer = XmlWriter.Create(stream, settings);

                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "http://schemas.datacontract.org/2004/07/EB.PayDirect.Hermes.Core.Facade");

                    var serializer = new XmlSerializer(typeof (T));
                    serializer.Serialize(writer, from, ns);
                    var length = stream.Length;
                    stream.Seek(0, SeekOrigin.Begin);

                    content = new StreamContent(stream);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                    content.LoadIntoBuffer();
                }
            }
            return content;
        }

        static public T DeserializeToEntity<T>(this HttpContent content)
        {
            if (content ==null || 
                content.Headers.ContentType == null ||
                content.Headers.ContentType.MediaType == null ||
                !content.Headers.ContentType.MediaType.Contains("xml"))
            {
                return default(T);
            }

            var reader = XmlReader.Create(content.ContentReadStream);
            var serializer = new XmlSerializer(typeof(T), new [] {typeof(Identity)});
            var entity = (T)serializer.Deserialize(reader);
            return entity;
        }
        
    }
}
