using Autofac;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.Business.Validator;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            
            builder.RegisterType<GroupService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.Repository = c.Context.Resolve<IGroupRepository>();
                    //c.Instance.Validator = c.Context.Resolve<GroupValidator>();
                });

            builder.RegisterType<LogService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.Repository = c.Context.Resolve<ILogRepository>();
                });

            builder.RegisterType<MessageService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.Repository = c.Context.Resolve<IMessageRepository>();
                    c.Instance.Validator = c.Context.Resolve<MessageValidator>();
                    c.Instance.GroupService = c.Context.Resolve<IGroupService>();
                    c.Instance.LogService = c.Context.Resolve<ILogService>();
                    c.Instance.RetryService = c.Context.Resolve<IRetryService>();
                    c.Instance.TopicService = c.Context.Resolve<ITopicService>();
                    c.Instance.SubscriptionService = c.Context.Resolve<ISubscriptionService>();

                });

            builder.RegisterType<RetryService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.Repository = c.Context.Resolve<IRetryRepository>();
                    c.Instance.LogService = c.Context.Resolve<ILogService>();
                });

            builder.RegisterType<SubscriptionService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.Repository = c.Context.Resolve<ISubscriptionRepository>();
                    c.Instance.Validator = c.Context.Resolve<SubscriptionValidator>();
                    c.Instance.GroupService = c.Context.Resolve<IGroupService>();
                    c.Instance.TopicService = c.Context.Resolve<ITopicService>();
                });

            builder.RegisterType<TopicService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.Repository = c.Context.Resolve<ITopicRepository>();
                    c.Instance.Validator = c.Context.Resolve<TopicValidator>();
                });
        }
    }
}