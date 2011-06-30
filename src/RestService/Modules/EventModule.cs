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
                                     Func<Type, IEnumerable> handlerFactory = t => (IEnumerable) cc.Resolve(typeof (IEnumerable<>).MakeGenericType(t));
                                     return new EventAggregator(handlerFactory);
                                 })
                    .As<IEventAggregator>()
                    .SingleInstance();
        }
    }
}