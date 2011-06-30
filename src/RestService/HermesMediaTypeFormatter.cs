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
        }
    }
}