What is Hermes?
===============
Hermes is a simple opensource middleware for communicating applications. Hermes follows the Publish/Subscribe messaging pattern where senders (Publishers) instead of send the messages directly to receivers (Subscribers), they publish the messages to a centralized repository (Hermes in this case) characterized into topics. Subscribers express interest in one or more topics.

How is Hermes different than any other Pub/Sub application?
===========================================================
The power of Hermes resides in its super-simple protocol. Hermes has a REST API to manage groups, topics and push messages that publishers can use. Subscribers poll messages from an atom feed.
Hermes has also a web portal that you can use to administrate topics, groups and to see some statistics. You can even push messages from the web portal.

Hermes is not Biztalk
=====================
Hermes is not a message broker but a Publish/Subscribe infrastructure. Hermes doesn't know what messages has receive what subscriber. Instead Hermes provides a paginated atom feed that subscribers can easily navigate. 

How does it work?
=================
Hermes uses mongodb to store messages, and the wcf web api. Despite that Hermes is built on the .Net Framework, the rest API could be used in almost all languages and devices.

Road map
========
*	Enhance the portal
*	Add support for long polling; even if we have some spikes of this, we are probabily open a new opensource project for doing this.
*	Create a real scenario application
*	Create more client examples in different technologies (javascript is the most important right now)
*	Add support for serialization/deserialization of messages in the .Net client.


Using the client library for .Net
=================================

This is an example of a chat application based on Hermes.Client.dll

Publisher:
----------
	static void Main()
	{
		const string uri = "http://localhost:6156";
		var hermesClient = new HermesClient(uri);

		var topic = hermesClient.TryCreateGroup("Chat Server")
								.TryCreateTopic("Weather Channel");

		Console.WriteLine("Publishing in topic \"{0}\" from group \"{1}\"", topic.Name, topic.Group.Name);
		
		string input;
		while (!string.IsNullOrEmpty(input = Console.ReadLine()))
		{
			topic.PostStringMessage(input);
			Console.WriteLine();
		}
	}

Subscriber:
-----------
	static void Main(string[] args)
	{
		const string uri = "http://localhost:6156";
		
		var hermesClient = new HermesClient(uri);
		var topic = hermesClient.TryCreateGroup("Chat Group")
								.TryCreateTopic("Weather Channel");
								
		using(var subscription = topic.PollFeed(2).Subscribe(Console.WriteLine))
		{
			Console.WriteLine("Subscribed to topic \"{0}\" from group \"{1}\"", topic.Name, topic.Group.Name);
			Console.ReadLine();
		}
	}

