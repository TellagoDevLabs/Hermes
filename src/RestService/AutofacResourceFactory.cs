using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using Autofac;
using Microsoft.ApplicationServer.Http.Description;

namespace TellagoStudios.Hermes.RestService
{
    public class AutofacResourceFactory : IResourceFactory
    {
        private readonly IContainer _container;

        public AutofacResourceFactory(IContainer container)
        {
            _container = container;
        }

        public object GetInstance(Type serviceType, InstanceContext instanceContext, HttpRequestMessage request)
        {
            // Begin lifetime scope when the service is instantiated.
            var lifetime = this._container.BeginLifetimeScope();
            // Hold the lifetime as an extension in the instance context
            instanceContext.Extensions.Add(new AutofactLifetimeExtension(lifetime));

            return lifetime.Resolve(serviceType);
        }

        public void ReleaseInstance(System.ServiceModel.InstanceContext instanceContext, object service)
        {
            var extension = instanceContext.Extensions.OfType<AutofactLifetimeExtension>().FirstOrDefault();
            // If we find our extension there, dispose it so the lifetime gets disposed.
            if (extension != null)
                extension.Dispose();
        }

        private class AutofactLifetimeExtension : IExtension<InstanceContext>, IDisposable
        {
            private ILifetimeScope lifetime;
            private bool isDisposed;

            public AutofactLifetimeExtension(ILifetimeScope lifetime)
            {
                this.lifetime = lifetime;
            }

            public void Attach(InstanceContext owner)
            {
            }

            public void Detach(InstanceContext owner)
            {
                Dispose();
            }

            public void Dispose()
            {
                if (this.isDisposed)
                    return;

                this.lifetime.Dispose();
                this.isDisposed = true;
            }
        }
    }
}