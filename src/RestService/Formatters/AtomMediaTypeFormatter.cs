using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http;

namespace TellagoStudios.Hermes.RestService.Formatters
{
    public class AtomMediaTypeFormatter : MediaTypeFormatter
    {
        public AtomMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/atom+xml"));
        }
        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            throw new NotImplementedException();
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            var formatter = new Atom10FeedFormatter((SyndicationFeed) value);
            using (var xmlWriter = XmlWriter.Create(stream))
            {
                formatter.WriteTo(xmlWriter);
                xmlWriter.Close();
            }
        }
        protected override bool OnCanReadType(Type type)
        {
            return false;
        }

        protected override bool OnCanWriteType(Type type)
        {
            return typeof (SyndicationFeed).IsAssignableFrom(type);
        }
    }
}