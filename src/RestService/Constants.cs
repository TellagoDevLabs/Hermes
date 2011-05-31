namespace TellagoStudios.Hermes.RestService
{
    public static class Constants
    {
        public const string HermesHostConfigurationObjectName = "HermesHostConfiguration";

        public class PrivateHeaders
        {
            public const string Prefix = "HUB_";
            public const string PromotedProperty = Prefix + "PromotedProperty_";
            public const string PromotedProperties = Prefix + "PromotedProperties";
        }

    }
}