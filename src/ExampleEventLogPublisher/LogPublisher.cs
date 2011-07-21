using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Diagnostics;
using System.Text;
using TellagoStudios.Hermes.Client;

namespace ExampleEventLogPublisher
{
    internal class LogPublisher
    {
        readonly Topic topic;
        readonly EventLogWatcher watcher;

        public LogPublisher(EventLog eventLog, Topic topic)
        {
            this.topic = topic;

            // Creates an EventLog watcher and subscribes to EventRecordWritten event
            var query = new EventLogQuery(eventLog.Log, PathType.LogName); 
            watcher = new EventLogWatcher(query); 
            watcher.EventRecordWritten += WatcherEventRecordWritten;
            watcher.Enabled = true;
        }

        void WatcherEventRecordWritten(object sender, EventRecordWrittenEventArgs e)
        {
            // This method is being invoked every time that a entry was written to the event log.
            try
            {
                using (var stream = new MemoryStream())
                {
                    // Serialize EventLog's entry as Xml
                    var writer = new StreamWriter(stream, Encoding.ASCII);
                    var xml = e.EventRecord.ToXml();
                    writer.Write(xml);
                    stream.Seek(0, SeekOrigin.Begin);

                    // Publish a new message
                    var urlToMessage = topic.PostMessage(stream, "application/xml");

                    // Prints new message's url
                    Utils.WriteOnScrollableFrame(urlToMessage);
                }
            }
            catch (Exception ex)
            {
                // Prints any exception
                Console.WriteLine(ex.ToString());
            }
        }
    }
}