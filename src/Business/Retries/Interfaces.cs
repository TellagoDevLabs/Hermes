using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Retries
{
    public interface ICreateRetryCommand
    {
        void Execute(Retry retry);
    }

    public interface IUpdateRetryCommand
    {
        void Execute(Retry retry);
    }

    public interface IDeleteRetryCommand
    {
        void Execute(Identity identity);
    }
}