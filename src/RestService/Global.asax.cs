using System;
using System.Configuration;
using System.Web.Routing;
using Autofac;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using TellagoStudios.Hermes.Business.Service;
using TellagoStudios.Hermes.Business.Validator;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.RestService
{
    public class Global : System.Web.HttpApplication
    {

        private void Initialize()
        {

            var cs = ConfigurationManager.ConnectionStrings["db.connectionString"];
            if (cs == null)
            {
                throw new ConfigurationErrorsException(
                    "A connection string names \"db.connectionString\" is missing at configuration file.");
            }

            #region IoC configuration

            var builder = new ContainerBuilder();

            #region Services

            builder.RegisterType<GroupService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                                 {
                                     c.Instance.Repository = c.Context.Resolve<IGroupRepository>();
                                     c.Instance.Validator = c.Context.Resolve<GroupValidator>();
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

            #endregion

            #region Validators

            builder.RegisterType<GroupValidator>()
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(c =>
                {
                    c.Instance.Repository = c.Context.Resolve<IGroupRepository>();
                    c.Instance.TopicService = c.Context.Resolve<ITopicService>();
                });

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

            #region Repositories
            
            builder.RegisterType<MongoDbMessageRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .WithParameter("connectionString", cs.ConnectionString);

            builder.RegisterType<MongoDbGroupRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .WithParameter("connectionString", cs.ConnectionString);

            builder.RegisterType<MongoDbTopicRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .WithParameter("connectionString", cs.ConnectionString);

            builder.RegisterType<MongoDbLogRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .WithParameter("connectionString", cs.ConnectionString);

            builder.RegisterType<MongoDbRetryRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .WithParameter("connectionString", cs.ConnectionString);

            builder.RegisterType<MongoDbSubscriptionRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .WithParameter("connectionString", cs.ConnectionString);

            #endregion

            #region Resources

            builder.RegisterType<GroupsResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<LogResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<MessageResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<SubscriptionResource>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<TopicsResource>().AsSelf().AsImplementedInterfaces();

            #endregion


            var container = builder.Build();
            
            #endregion
            
            #region Initialize Routes

            IHttpHostConfigurationBuilder config;
            if (!container.TryResolve(out config))
            {
                config = HttpHostConfiguration.Create()
                    .SetResourceFactory(new AutofacResourceFactory(container));
            }

            RouteTable.Routes.MapServiceRoute<TopicsResource>(Business.Constants.Routes.Topics, config);
            RouteTable.Routes.MapServiceRoute<MessageResource>(Business.Constants.Routes.Messages, config);
            RouteTable.Routes.MapServiceRoute<GroupsResource>(Business.Constants.Routes.Groups, config);
            RouteTable.Routes.MapServiceRoute<SubscriptionResource>(Business.Constants.Routes.Subscriptions, config);
            RouteTable.Routes.MapServiceRoute<LogResource>(Business.Constants.Routes.Log, config);

            #endregion

            #region Initial Process of Retries queue

            var retryService = container.Resolve<IRetryService>();
            retryService.ProcessRetries();  
            
            #endregion

        }

        void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            Initialize();
        }        
    }
}
