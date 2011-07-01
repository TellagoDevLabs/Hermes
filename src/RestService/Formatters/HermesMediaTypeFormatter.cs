using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.RestService.Formatters
{
    public class HermesMediaTypeFormatter : XmlMediaTypeFormatter
    {
        public HermesMediaTypeFormatter()
        {
            XmlSerializerNamespaces.Add("", XmlNamespaces.Default);
        }
        protected override bool OnCanWriteType(System.Type type)
        {
            return !typeof(SyndicationFeed).IsAssignableFrom(type) && base.OnCanWriteType(type);
        }
    }
}