using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Service
{
    public interface ILogService
    {
        LogEntry Create(LogEntry entry);
        LogEntry Get(Guid id);
        void Truncate();
        IEnumerable<LogEntry> Find(string query, int? skip = null, int? limit = null);
    }
}