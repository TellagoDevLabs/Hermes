using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;

namespace TellagoStudios.Hermes.RestService.Formatters
{
    public class JsonMediaTypeFormatter : MediaTypeFormatter
    {
        public JsonMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            return Facade.Serialization.JsonSerializer.Deserialize(type, stream);
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            Facade.Serialization.JsonSerializer.Serialize(value, stream);
        }
        protected override bool OnCanReadType(Type type)
        {
            return true;
        }

        protected override bool OnCanWriteType(Type type)
        {
            return true;
        }
    }
}