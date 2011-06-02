using System;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Service;

namespace TellagoStudios.Hermes.Business
{
    public static class LogServiceExtensions
    {
        public static LogEntry LogInformation(this ILogService service, string message)
        {
            return service.Create(new LogEntry { UtcTs = DateTime.UtcNow, Type = LogEntryType.Information, Message = message });
        }

        public static LogEntry LogWarning(this ILogService service, string message)
        {
            return service.Create(new LogEntry { UtcTs = DateTime.UtcNow, Type = LogEntryType.Warning, Message = message });
        }

        public static LogEntry LogError(this ILogService service, string message)
        {
            return service.Create(new LogEntry { UtcTs = DateTime.UtcNow, Type = LogEntryType.Error, Message = message });
        }

        public static LogEntry LogError(this ILogService service, string message, Exception ex)
        {
            return service.Create(new LogEntry { UtcTs = DateTime.UtcNow, Type = LogEntryType.Error, Message = string.Format("{0}\r\n{1}\r\n{2}", message, ex, ex.StackTrace) });
        }

    }
}
