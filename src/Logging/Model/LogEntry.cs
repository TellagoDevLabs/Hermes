using System;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Logging.Model
{
    public class LogEntry
    {
        public Identity? Id { get; set; }
        public LogEntryType? Type { get; set; }        
        public DateTime UtcTs { get; set; }
        public string Message { get; set; }       
    }
}
