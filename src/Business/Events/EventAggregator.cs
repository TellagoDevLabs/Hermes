using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TellagoStudios.Hermes.Business.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Func<Type, IEnumerable> handlerFactory;
        private readonly ISubject<object> events = new Subject<object>();

        public EventAggregator(Func<Type, IEnumerable> handlerFactory)
        {
            this.handlerFactory = handlerFactory;
        }

        public void Raise<T>(T @event)
        {
            events.OnNext(@event);
            var handlers = handlerFactory(@event.GetType())
                                .OfType<IHandler<T>>();
            foreach (var handler in handlers)
            {
                handler.Handle(@event);
            }
        }

        public void Subscribe<T>(IEventHandler<T> handler)
        {
            events.OfType<T>().Subscribe(handler.Handle);
        }

        public void Subscribe(IEventHandler handler)
        {
            events.Where(e => e != null && e.GetType() == handler.Type)
                  .Subscribe(handler.Handle);
        }
    }

    public interface IHandler<in T>
    {
        void Handle(T @event);
    }
}
