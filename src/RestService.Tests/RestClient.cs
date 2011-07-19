using System;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using TellagoStudios.Hermes.Facade;
using TellagoStudios.Hermes.Facade.Serialization;

namespace RestService.Tests
{
    public class RestClient
    {
        public enum SerializationType
        {
            Xml,
            Json
        }

        public Uri Url { get; protected set; }
        private SerializationType serializationType = SerializationType.Xml;

        public RestClient(Uri serviceUrl, SerializationType serializationType )
        {
            if (serviceUrl == null)
                throw new ArgumentNullException("serviceUrl");

            Url = serviceUrl;
            this.serializationType = serializationType;
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

        public void ExecutePut<T>(string operation, T entity, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            DoPut(operation, entity, null, expectedCode);
        }

        public U ExecutePut<T, U>(string operation, T entity, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            U instance = default(U);

            DoPut(operation, entity, response => instance = response.Content.DeserializeToEntity<U>(), expectedCode);

            return instance;
        }

        public Uri ExecutePost<T>(string operation, T entity, HttpStatusCode expectedCode = HttpStatusCode.Created)
        {
            Uri location = null;
            DoPost<T>(operation, entity, response => location = response.Headers.Location, expectedCode);
            return location;
        }

        public void ExecuteDelete(string operation, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
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
            var client = new HttpClient(url);
            switch (serializationType)
            {
                case SerializationType.Xml:
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    break;
                case SerializationType.Json:
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    break;
            }
            return client;
        }

        private void DoPost<T>(string operation, T entity, Action<HttpResponseMessage> action, HttpStatusCode expectedCode = HttpStatusCode.Created)
        {
            using (var client = CreateClient(Url))
            {
                var content = entity.SerializeToContent(serializationType);
                using (HttpResponseMessage response = client.Post(Combine(operation), content))
                {
                    Assert.AreEqual(expectedCode, response.StatusCode);
                    if (action != null)
                    {
                        action(response);
                    }
                }
            }
        }

        private void DoPut<T>(string operation, T entity, Action<HttpResponseMessage> action, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            using (var client = CreateClient(Url))
            {
                var content = entity.SerializeToContent(serializationType);
                using (HttpResponseMessage response = client.Put(Combine(operation), content))
                {
                    Assert.AreEqual(expectedCode, response.StatusCode);
                    if (action != null)
                    {
                        action(response);
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
        static public HttpContent SerializeToContent<T>(this T from, RestClient.SerializationType serializationType)
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
                    switch (serializationType)
                    {
                        case RestClient.SerializationType.Xml:
                            var xmlSettings = new XmlWriterSettings {OmitXmlDeclaration = true};
                            var xmlWriter = XmlWriter.Create(stream, xmlSettings);

                            var ns = new XmlSerializerNamespaces();
                            ns.Add("", "http://schemas.datacontract.org/2004/07/EB.PayDirect.Hermes.Core.Facade");

                            var xmlSerializer = new XmlSerializer(typeof (T));
                            xmlSerializer.Serialize(xmlWriter, from, ns);
                            break;
                        case RestClient.SerializationType.Json:
                            JsonSerializer.Serialize(from, stream);
                            break;
                    };

                    var length = stream.Length;
                    stream.Seek(0, SeekOrigin.Begin);

                    content = new StreamContent(stream);
                    switch (serializationType)
                    {
                        case RestClient.SerializationType.Xml:
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                            break;
                        case RestClient.SerializationType.Json:
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            break;

                    }
                    content.LoadIntoBuffer();
                }
            }
            return content;
        }

        static public T DeserializeToEntity<T>(this HttpContent content)
        {
            var entity = default(T);

            if (content == null || 
                content.Headers.ContentType == null ||
                content.Headers.ContentType.MediaType == null)
            {
                return entity;
            }

            if (content.Headers.ContentType.MediaType.Contains("xml"))
            {

                var xmlReader = XmlReader.Create(content.ContentReadStream);
                var xmlSerializer = new XmlSerializer(typeof (T), new[] {typeof (Identity)});
                entity = (T)xmlSerializer.Deserialize(xmlReader);
            }
            else if (content.Headers.ContentType.MediaType.Contains("json"))
            {
                entity = JsonSerializer.Deserialize<T>(content.ContentReadStream);
            }
            return entity;
        }
            
    }
}
