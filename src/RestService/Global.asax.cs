using System;
using System.Configuration;
using System.Linq;
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

            
            var builder = new ContainerBuilder();
            
            typeof(Global).Assembly
                          .GetTypes()
                          .Where(t => typeof(Module).IsAssignableFrom(t) && !t.IsAbstract)
                          .Select(Activator.CreateInstance).OfType<Module>()
                          .ToList().ForEach(builder.RegisterModule);

            var container = builder.Build();
            
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
