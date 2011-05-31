using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business;

namespace TellagoStudios.Hermes.Business.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public string[] Messages { get; private set; }

        public ValidationException() 
        {
        }

        public ValidationException(string message) 
            : base(message)
        {
            var messages = new List<string>();
            if (!string.IsNullOrWhiteSpace(message))
            {
                messages.Add(message);
            }
            Messages = messages.ToArray();
        }

        public ValidationException(string message, params object[] args) 
            : this(string.Format(message, args))
        {
        }

        public ValidationException(IEnumerable<string> messages) 
            : base(ConcatAllMessages(messages))
        {
            Messages = messages
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToArray();
        }

        static private string ConcatAllMessages(IEnumerable<string> messages)
        {
            Guard.Instance.ArgumentNotNullOrEmpty(()=>messages, messages);

            return messages
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Aggregate((a, b) => a + Environment.NewLine + b);
        }
    }
}
