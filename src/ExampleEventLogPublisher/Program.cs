using System;
using System.Diagnostics;
using System.Linq;
using TellagoStudios.Hermes.Client;

namespace ExampleEventLogPublisher
{
    class Program
    {
        static void Main()
        {
            try
            {
                var url = "http://localhost:6156";

                Console.WriteLine("This application must run with administrator privilege.");
                Console.WriteLine("Connecting to Hermes at " + url);
                Console.WriteLine();

                // Connect to Hermes
                var client = new HermesClient(url);

                // Get or create a group that will contains all the topics
                var group = client.TryCreateGroup("Windows EventLog");

                // Create a publisher for each Event Log that this machine has
                var publishers = EventLog.GetEventLogs() // Retrieve all event logs
                    .Select(el =>
                                {
                                    // Get or create the topic for the event log
                                    var topic = group.TryCreateTopic(el.Log);

                                    // Print the topic's feed Url
                                    Console.WriteLine("Topic: {0} {1}", topic.Name, topic.GetLinkForFeed());

                                    // Create a publisher instance
                                    return new LogPublisher(el, topic);
                                })
                    .ToArray(); // Keep all publisher instances

                Console.WriteLine();
                Console.WriteLine("Connected and listening {0} event logs", publishers.Length);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                // Shows any exception
                Console.WriteLine();
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
