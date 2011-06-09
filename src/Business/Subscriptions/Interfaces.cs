using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Subscriptions
{
    public interface ICreateSubscriptionCommand
    {
        void Execute(Subscription subscription);
    }

    public interface IUpdateSubscriptionCommand
    {
        void Execute(Subscription subscription);
    }

    public interface IDeleteSubscriptionCommand
    {
        void Execute(Identity identity);
    }
}