using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using TellagoStudios.Hermes.RestService.Pushing;
using TellagoStudios.Hermes.RestService.Resources;
using TellagoStudios.Hermes.Business.Events;
using Module = Autofac.Module;
using ResourceLocation = TellagoStudios.Hermes.RestService.Resources.ResourceLocation;

namespace TellagoStudios.Hermes.RestService
{
    public class Global : System.Web.HttpApplication
    {
        private IEventAggregator _aggregator;

        private void Initialize()
        {
            var cs = ConfigurationManager.ConnectionStrings["db.connectionString"];
            if (cs == null)
            {
                throw new ConfigurationErrorsException(
                    "A connection string names \"db.connectionString\" is missing at configuration file.");
            }
            
            var builder = new ContainerBuilder();
            
            typeof(Global).Assembly
                          .GetTypes()
                          .Where(t => typeof(Module).IsAssignableFrom(t) && !t.IsAbstract)
                          .Select(Activator.CreateInstance).OfType<Module>()
                          .ToList().ForEach(builder.RegisterModule);

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

         
            #region Initialize Routes MVC

            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            RouteTable.Routes.MapRoute(
                "Default", // Route name
                "admin/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            //Special Route for css so that images are relative to it
            RouteTable.Routes.MapRoute("css",
                           "public/css/{filename}.css",
                           new { controller = "Css", action = "Index" },
                           new[] { "TellagoStudios.Hermes.RestService.Controllers" });

            #endregion            
            
            #region Initialize Routes REST

            IHttpHostConfigurationBuilder config;
            if (!container.TryResolve(out config))
            {
                config = HttpHostConfiguration.Create()
                    .SetResourceFactory(new AutofacResourceFactory(container))
                    .SetOperationHandlerFactory(container.Resolve<HttpOperationHandlerFactory>());
            }

            RouteTable.Routes.MapServiceRoute<TopicsResource>(Constants.Routes.Topics, config);
            RouteTable.Routes.MapServiceRoute<MessageResource>(Constants.Routes.Messages, config);
            RouteTable.Routes.MapServiceRoute<GroupsResource>(Constants.Routes.Groups, config);
            RouteTable.Routes.MapServiceRoute<SubscriptionResource>(Constants.Routes.Subscriptions, config);
            RouteTable.Routes.MapServiceRoute<FeedResource>(Constants.Routes.Feed, config);

            #endregion

            ResourceLocation.BaseAddress = new Uri(ConfigurationManager.AppSettings["baseAddress"]);

            #region Initial Process of Retries queue

            var retryService = container.Resolve<IRetryService>();
            retryService.ProcessRetries();

            _aggregator = container.Resolve<IEventAggregator>();
            var handlers = container.Resolve<IEnumerable<IEventHandler>>();
            foreach (var handler in handlers)
            {
                _aggregator.Subscribe(handler);
            }

            #endregion
        }

        void Application_Start(object sender, EventArgs e)
        {
            Initialize();
        }
    }
}
