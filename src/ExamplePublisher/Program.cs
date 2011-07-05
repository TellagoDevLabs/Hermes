using System;
using System.IO;
using System.Linq;
using System.Text;
using TellagoStudios.Hermes.Client;

namespace ExamplePublisher
{
    class Program
    {
        static void Main()
        {
            const string uri = "http://localhost:6156";
            var hermesClient = new HermesClient(uri);

            var messages = Enumerable.Range(0, 100);
            foreach (var groupNumber in messages)
            {
                var groupName = string.Format("Group {0}", groupNumber);
                Console.WriteLine("Creating group \"{0}\"", groupName);

                hermesClient.CreateGroup(groupName);

                Console.WriteLine("Creado! Presione enter para crear otro...");
                Console.ReadLine();
            }


            //var topic = CreateOrGetTopic(hermesClient);
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine("Publishing in topic \"{0}\" from group \"{1}\"", topic.Name, topic.Group.Name);
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine();
            //string read;
            //while (!string.IsNullOrEmpty(read = Console.ReadLine()))
            //{
                
            //    var location = topic.PostStringMessage(read);
            //    Console.WriteLine();
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Posted message {0}.", read);
            //    Console.WriteLine("You can get the message in:");
            //    Console.WriteLine(location);
            //    Console.ForegroundColor = ConsoleColor.White;
            //    Console.WriteLine();
            //}
        }

        public static Topic CreateOrGetTopic(HermesClient hermesClient)
        {
            var result = hermesClient.GetGroups().FirstOrDefault(g => g.Name == "Test");
            if (result != null)
            {
                var chatTopic = result.GetAllTopics().FirstOrDefault(t => t.Name == "Chat");
                return chatTopic;
            }
            return hermesClient.CreateGroup("Test").CreateTopic("Chat");
        }
    }
}
