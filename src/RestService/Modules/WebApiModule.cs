using Autofac;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using TellagoStudios.Hermes.RestService.Formatters;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpOperationHandlerFactory>()
                   .AsSelf().WithParameter("formatters", new MediaTypeFormatter[]
                                                             {
                                                                 new AtomMediaTypeFormatter(), 
                                                                 new HermesMediaTypeFormatter(),
                                                                 new Formatters.JsonMediaTypeFormatter()
                                                             })
                   .SingleInstance();
        }
    }
}