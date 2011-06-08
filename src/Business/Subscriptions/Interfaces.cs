using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Subscriptions
{
    public interface ICreateSubscriptionCommand
    {
        void Execute(Subscription Subscription);
    }

    public interface IUpdateSubscriptionCommand
    {
        void Execute(Subscription Subscription);
    }

    public interface IDeleteSubscriptionCommand
    {
        void Execute(Identity identity);
    }
}