using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.RestService
{
    public class HermesMediaTypeFormatter : XmlMediaTypeFormatter
    {
        public HermesMediaTypeFormatter()
        {
            XmlSerializerNamespaces.Add("", XmlNamespaces.Default);
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.hermes+xml") { CharSet = "utf-8" });
        }
    }
}