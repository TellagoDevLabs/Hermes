using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Service
{
    public interface IRetryService
    {
        Retry Add(Retry message);
        void ProcessRetries();
        bool IsRunning { get; }
    }
}