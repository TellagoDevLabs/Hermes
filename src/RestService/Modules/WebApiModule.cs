using Autofac;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(WebApiModule).Assembly)
                .Where(t => typeof(MediaTypeFormatter).IsAssignableFrom(t))
                .As<MediaTypeFormatter>();

            builder.RegisterType<HttpOperationHandlerFactory>()
                   .AsSelf()
                   .SingleInstance();
        }
    }
}