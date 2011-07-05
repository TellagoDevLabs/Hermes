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
            var topic = CreateOrGetTopic(hermesClient);
            var subscription = topic.PollFeed(2)
                                    .Subscribe(Console.WriteLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Subscribed to topic \"{0}\" from group \"{1}\"", topic.Name, topic.Group.Name);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.ReadLine();
            subscription.Dispose();
        }

        public static Topic CreateOrGetTopic(HermesClient hermesClient)
        {
            var result = hermesClient.GetGroups().FirstOrDefault(g => g.Name == "Test");
            if (result != null)
            {
                var chatTopic = result.GetAllTopics().FirstOrDefault(t => t.Name == "Chat");
                return chatTopic;
            }
            else
            {
                return hermesClient.CreateGroup("Test").CreateTopic("Chat");
            }
        }
    }
}
