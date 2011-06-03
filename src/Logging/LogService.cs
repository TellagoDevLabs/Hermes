using System;
using System.Collections.Generic;
using System.Diagnostics;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Logging
{
    public class LogService : ILogService
    {
        public ILogRepository Repository { get; set; }

        public LogEntry Create(LogEntry entry)
        {
            Guard.Instance.ArgumentNotNull(()=>entry, entry);

            try
            {
                return Repository.Create(entry);
            }
            catch(Exception ex)
            {
                // swallow the exception, logging should never fail
                Debug.WriteLine(string.Format("Error trying to log, exception will not be propagated.\r\n{0}\r\n{1}" + ex, ex.StackTrace));
                return null;
            }
        }

        public LogEntry Get(Identity id)
        {
            return Repository.Get(id);
        }

        public void Truncate()
        {                        
            Repository.Truncate();
        }

        public IEnumerable<LogEntry> Find(string query, int? skip = null, int? limit = null)
        {
            Guard.Instance
                .ArgumentValid(()=>skip, () => (skip.HasValue && skip.Value < 0))
                .ArgumentValid(()=>limit, () => (limit.HasValue && limit.Value <= 0));

            return Repository.Find(query, skip, limit);
        }
    }
}