using System;

namespace TellagoStudios.Hermes.Business.Model
{
    public class Callback
    {
        public Uri Url { get; set; }
        public CallbackKind Kind { get; set; }
    }
}