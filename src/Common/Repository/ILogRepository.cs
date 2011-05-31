using System;
using TellagoStudios.Hermes.Common.Model;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Common.Repository
{
    public interface ILogRepository
    {
        LogEntry Create(LogEntry entry);
        LogEntry Get(Guid id);
        void Truncate();
        IEnumerable<LogEntry> Find(string query, int? skip, int? limit);
    }
}