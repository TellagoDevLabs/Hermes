using System;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Service
{
    public static class LogServiceExtensions
    {
        static public LogEntry LogInformation(this ILogService service, string message)
        {
            return service.Create(new LogEntry { UtcTs = DateTime.UtcNow, Type = LogEntryType.Information, Message = message });
        }

        static public LogEntry LogWarning(this ILogService service, string message)
        {
            return service.Create(new LogEntry { UtcTs = DateTime.UtcNow, Type = LogEntryType.Warning, Message = message });
        }

        static public LogEntry LogError(this ILogService service, string message)
        {
            return service.Create(new LogEntry { UtcTs = DateTime.UtcNow, Type = LogEntryType.Error, Message = message });
        }

        static public LogEntry LogError(this ILogService service, string message, Exception ex)
        {
            return service.Create(new LogEntry { UtcTs = DateTime.UtcNow, Type = LogEntryType.Error, Message = string.Format("{0}\r\n{1}\r\n{2}", message, ex, ex.StackTrace) });
        }

    }
}
