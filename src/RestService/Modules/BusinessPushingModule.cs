using Autofac;
using TellagoStudios.Hermes.RestService.Pushing;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class BusinessPushingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RetryService>()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterType<NewMessagePusher>()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

        }
    }
}