using System.Configuration;
using Autofac;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using TellagoStudios.Hermes.DataAccess.Queries;

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

            builder.RegisterGeneric(typeof (Repository<>))
                    .As(typeof (IRepository<>))
                    .WithParameter("connectionString", cs.ConnectionString);

            builder.RegisterAssemblyTypes(typeof (MongoDbRepository).Assembly)
                .Where( t =>  t.Namespace.StartsWith("TellagoStudios.Hermes.DataAccess.Queries") && !t.IsAbstract && !t.IsInterface)
                .WithParameter("connectionString", cs.ConnectionString)
                .AsImplementedInterfaces();
        }
    }
}