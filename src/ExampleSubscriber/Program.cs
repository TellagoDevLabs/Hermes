using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TellagoStudios.Hermes.Client;

namespace ExampleSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            const string uri = "http://localhost:6156";
            
            var hermesClient = new HermesClient(uri);
            
            var topic = hermesClient.TryCreateGroup("Chat Server")
                                    .TryCreateTopic("Weather Channel");

            using(var subscription = topic.PollFeed(2).Subscribe(Console.WriteLine))
            {
                Console.ReadLine();
                subscription.Dispose();    
            }
        }

        
    }
}
