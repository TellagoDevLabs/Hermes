using Autofac;
using TellagoStudios.Hermes.Business.Exceptions;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class CommandsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof (EntityNotFoundException).Assembly)
                .Where(t => t.Name.EndsWith("Command") && !t.IsAbstract && !t.IsInterface)
                .AsImplementedInterfaces();
        }
    }
}