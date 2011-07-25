using System;

namespace TellagoStudios.Hermes.Client
{
    public class Message<T>
    {
        public Uri Url { get; set; }    
        public T Data { get; set; }
    }
}