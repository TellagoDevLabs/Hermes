using System;

namespace TellagoStudios.Hermes.Business.Events
{
    public interface IEventHandler
    {
        Type Type { get; }
        void Handle(object @event);
    }

    public interface IEventHandler<T> : IEventHandler
    {
        void Handle(T @event);
    }
}