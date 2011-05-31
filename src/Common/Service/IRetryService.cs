using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Service
{
    public interface IRetryService
    {
        Retry Add(Retry message);
        void ProcessRetries();
        bool IsRunning { get; }
    }
}