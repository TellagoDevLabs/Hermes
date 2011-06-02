using System;
using System.Linq;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Business.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly ISubject<object> events = new Subject<object>();

        public void Raise(object @event)
        {
            events.OnNext(@event);
        }

        public void Subscribe<T>(IEventHandler<T> handler)
        {
            events.OfType<T>().Subscribe(handler.Handle);
        }

        public void Subscribe(IEventHandler handler)
        {
            events.Where(e => e != null && e.GetType() == handler.Type).Subscribe(handler.Handle);
        }
    }
}
