using Autofac;
using TellagoStudios.Hermes.RestService.Resources;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class ResourcesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<GroupResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<GroupsResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<MessageResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<SubscriptionResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<TopicResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<TopicsResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<FeedResource>().AsSelf().AsImplementedInterfaces();
        }
    }
}