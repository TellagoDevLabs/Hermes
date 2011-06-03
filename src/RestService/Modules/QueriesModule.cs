using System.Configuration;
using Autofac;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class QueriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var cs = ConfigurationManager.ConnectionStrings["db.connectionString"];
            if (cs == null)
            {
                throw new ConfigurationErrorsException(
                    "A connection string names \"db.connectionString\" is missing at configuration file.");
            }

            base.Load(builder);
            builder.RegisterAssemblyTypes(typeof (MongoDbRepository).Assembly)
                .Where( t =>  t.Namespace.StartsWith("TellagoStudios.Hermes.DataAccess.Queries") && !t.IsAbstract && !t.IsInterface)
                .WithParameter("connectionString", cs.ConnectionString)
                .AsImplementedInterfaces();
        }
    }
}