using System;

namespace TellagoStudios.Hermes.Common.Model
{
    public class Callback
    {
        public Uri Url { get; set; }
        public CallbackKind Kind { get; set; }
    }
}