using System;

namespace TellagoStudios.Hermes.Common.Model
{
    public class LogEntry
    {
        public Guid? Id { get; set; }
        public LogEntryType? Type { get; set; }        
        public DateTime UtcTs { get; set; }
        public string Message { get; set; }       
    }
}
