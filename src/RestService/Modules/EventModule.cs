using System;
using System.Collections;
using System.Collections.Generic;
using Autofac;
using TellagoStudios.Hermes.Business.Events;
namespace TellagoStudios.Hermes.RestService.Modules
{
    public class EventModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register(cc =>
                                 {
                                     var context = cc.Resolve<IComponentContext>();
                                     Func<Type, IEnumerable> handlerFactory = t => (IEnumerable)context.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
                                     return new EventAggregator(handlerFactory);
                                 })
                    .As<IEventAggregator>()
                    .SingleInstance();
        }
    }
}