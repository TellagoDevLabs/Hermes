using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TellagoStudios.Hermes.Logging
{

    public class MongoTraceListener : TraceListener
    {
        private readonly ILogService _logService;

        public MongoTraceListener(ILogService logService)
        {
            _logService = logService;
        }

        public override void Write(string message)
        {
            this.TraceEvent(null, "n/a", TraceEventType.Information, 0, message);
        }

        public override void WriteLine(string message)
        {
            this.TraceEvent(null, "n/a", TraceEventType.Information, 0, message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            LogEntryType entryType;
            switch (eventType)
            {
                case TraceEventType.Information:
                    entryType = LogEntryType.Information;
                    break;
                case TraceEventType.Error:
                    entryType = LogEntryType.Error;
                    break;
                case TraceEventType.Warning:
                    entryType = LogEntryType.Warning;
                    break;
                default:
                    entryType = LogEntryType.Information;
                    break;
            }

            _logService.Create(new LogEntry {Message = message, Type = entryType, UtcTs = DateTime.UtcNow});
        }
    }
}
