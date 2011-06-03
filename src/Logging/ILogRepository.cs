using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Logging.Model;

namespace TellagoStudios.Hermes.Logging
{
    public interface ILogRepository
    {
        LogEntry Create(LogEntry entry);
        LogEntry Get(Identity id);
        void Truncate();
        IEnumerable<LogEntry> Find(string query, int? skip, int? limit);
    }
}