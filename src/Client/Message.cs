using System;

namespace TellagoStudios.Hermes.Client
{
    /// <summary>
    /// Topic's message
    /// </summary>
    /// <typeparam name="T">Type of message's content</typeparam>
    public class Message<T>
    {
        /// <summary>
        /// Message's url
        /// </summary>
        public Uri Url { get; set; }
    
        /// <summary>
        /// Messages's content
        /// </summary>
        public T Data { get; set; }
    }
}