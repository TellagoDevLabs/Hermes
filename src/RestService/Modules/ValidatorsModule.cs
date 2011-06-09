using Autofac;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.Business.Validator;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class ValidatorsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            #region Validators

            builder.RegisterType<MessageValidator>()
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.TopicService = c.Context.Resolve<ITopicService>();
                });

            builder.RegisterType<TopicValidator>()
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.TopicRepository = c.Context.Resolve<ITopicRepository>();
                });

            #endregion
        }
    }
}