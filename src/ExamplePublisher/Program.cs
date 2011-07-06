using System;
using TellagoStudios.Hermes.Client;

namespace ExamplePublisher
{
    class Program
    {
        static void Main()
        {
            const string uri = "http://localhost:6156";
            var hermesClient = new HermesClient(uri);

            var topic = hermesClient.TryCreateGroup("Chat Server")
                                    .TryCreateTopic("Weather Channel");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Publishing in topic \"{0}\" from group \"{1}\"", topic.Name, topic.Group.Name);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            string input;
            while (!string.IsNullOrEmpty(input = Console.ReadLine()))
            {

                var location = topic.PostStringMessage(input);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Posted message {0}.", input);
                Console.WriteLine("You can get the message in:");
                Console.WriteLine(location);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
        }
    }
}
