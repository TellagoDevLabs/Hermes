using Autofac;
using TellagoStudios.Hermes.Business.Events;
namespace TellagoStudios.Hermes.RestService.Modules
{
    public class EventModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<EventAggregator>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}