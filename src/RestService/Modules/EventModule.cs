using System;
using System.Collections;
using System.Collections.Generic;
using Autofac;
using TellagoStudios.Hermes.Business.Events;
using TellagoStudios.Hermes.Business.Feeds;

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
                                     Func<Type, IEnumerable> handlerFactory = t => (IEnumerable)context.Resolve(typeof(IEnumerable<>)
                                                                                                                    .MakeGenericType(typeof(IHandler<>)
                                                                                                                                         .MakeGenericType(t)));
                                     return new EventAggregator(handlerFactory);
                                 })
                    .As<IEventAggregator>()
                    .SingleInstance();

            builder.RegisterType<AddMessageToFeedHandler>()
                .As<IHandler<NewMessageEvent>>()
                .Keyed<IHandler<NewMessageEvent>>("AddNewMessageToFeed")
                .SingleInstance();
        }
    }
}