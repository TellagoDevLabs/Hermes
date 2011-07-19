using System;
using System.Linq;
using System.ServiceModel;
using Autofac;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using NUnit.Framework;
using TellagoStudios.Hermes.RestService.Formatters;
using TellagoStudios.Hermes.RestService.Resources;

namespace RestService.Tests
{
    [TestFixture]
    public abstract class ResourceBaseFixture
    {
        protected Uri baseUri = new Uri("http://localhost:8000");
        protected RestClient client;
        protected HttpConfigurableServiceHost host;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            // Creates a new Spring context
            var builder = new ContainerBuilder();

            PopulateApplicationContext(builder);

            var config = HttpHostConfiguration.Create()
                .AddFormatters(new AtomMediaTypeFormatter())
                .SetResourceFactory(new AutofacResourceFactory(builder.Build()));

            var type = GetServiceType();
            host = new HttpConfigurableServiceHost(type, config, baseUri);
            host.Open();

            ResourceLocation.BaseAddress = baseUri;

            // Create client instance 
            client = new RestClient(baseUri);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            if (host != null)
            {
                host.Close();
                host = null;
            }
        }

        protected abstract void PopulateApplicationContext(ContainerBuilder builder);
        protected abstract Type GetServiceType();
    }
}
