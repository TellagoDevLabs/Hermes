namespace TellagoStudios.Hermes.Business.Events
{
    public interface IEventAggregator
    {
        void Raise<T>(T @event);
        void Subscribe(IEventHandler handler);
        void Subscribe<T>(IEventHandler<T> handler);
    }
}
