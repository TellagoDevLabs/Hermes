using Autofac;
using TellagoStudios.Hermes.RestService.Pushing;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RetryService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
                //.OnActivated(c =>
                //{
                //    c.Instance.CreateRetryCommand = c.Context.Resolve<ICreateRetryCommand>();
                //    c.Instance.UpdateRetryCommand = c.Context.Resolve<IUpdateRetryCommand>();
                //    c.Instance.DeleteRetryCommand = c.Context.Resolve<IDeleteRetryCommand>();
                //    c.Instance.GenericJsonPagedQuery = c.Context.Resolve<IGenericJsonPagedQuery>();
                    
                //});
        }
    }
}