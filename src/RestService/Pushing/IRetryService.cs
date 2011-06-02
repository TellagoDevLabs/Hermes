using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Pushing
{
    public interface IRetryService
    {
        Retry Add(Retry message);
        void ProcessRetries();
        bool IsRunning { get; }
    }
}