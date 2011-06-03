using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Logging
{
    public static class IdentityExtensions
    {
        public static Identity ToModel(this Facade.Identity from)
        {
            if (from.IsEmpty) return Identity.Empty;

            return new Identity((string) from);
        }

        public static Identity? ToModel(this Facade.Identity? from)
        {
            if (from == null) return null;

            return from.Value.ToModel();
        }

        public static Facade.Identity ToFacade(this Identity from)
        {
            return (Facade.Identity)((string)from);
        }

        public static Facade.Identity? ToFacade(this Identity? from)
        {
            if (from == null) return null;

            return from.Value.ToFacade();
        }
    }
}