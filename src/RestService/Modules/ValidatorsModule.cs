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
                    c.Instance.GroupService = c.Context.Resolve<IGroupService>();
                    c.Instance.TopicService = c.Context.Resolve<ITopicService>();
                    c.Instance.SubscriptionService = c.Context.Resolve<ISubscriptionService>();
                });

            builder.RegisterType<SubscriptionValidator>()
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.GroupService = c.Context.Resolve<IGroupService>();
                    c.Instance.TopicService = c.Context.Resolve<ITopicService>();
                    c.Instance.Repository = c.Context.Resolve<ISubscriptionRepository>();
                });

            builder.RegisterType<TopicValidator>()
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.GroupService = c.Context.Resolve<IGroupService>();
                    c.Instance.TopicRepository = c.Context.Resolve<ITopicRepository>();
                });

            #endregion
        }
    }
}