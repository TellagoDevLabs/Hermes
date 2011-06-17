namespace TellagoStudios.Hermes.RestService
{
    public static class Messages
    {
        public const string CallbackKindUnknown = "The callback's kind '{0}' is unknown.";
        public const string InvalidCallbackFormat = "The callback property must meet the Uri format";
        public const string InvalidHeader = "The HTTP header is invalid. Name: {0}  Value: {1}";
        public const string TargetKindUnknown = "The target's kind '{0}' is unknown.";
        public const string ResourceLocationBaseAddressIsNull = "The ResourceLocation's BaseAddress property returns null.";
    }
}