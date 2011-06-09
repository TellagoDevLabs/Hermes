using System.Configuration;
using Autofac;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace TellagoStudios.Hermes.RestService.Modules
{
    public class RepositoriesModule : Module
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

            builder.RegisterType<MongoDbRetryRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .WithParameter("connectionString", cs.ConnectionString);
        }
    }
}