using System;
using TellagoStudios.Hermes.Business.Model;
using System.Collections.Generic;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface ILogRepository
    {
        LogEntry Create(LogEntry entry);
        LogEntry Get(Identity id);
        void Truncate();
        IEnumerable<LogEntry> Find(string query, int? skip, int? limit);
    }
}