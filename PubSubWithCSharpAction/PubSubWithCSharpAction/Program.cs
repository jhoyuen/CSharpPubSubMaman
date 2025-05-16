// See https://aka.ms/new-console-template for more information
using PubSubWithCSharpAction;

Console.WriteLine("Hello, World!");

Publisher publisher = new Publisher();
Subscriber subscriber = new Subscriber();
Subscriber subscriber2 = new Subscriber();

// Subscribe the subscribers
publisher.NotificationEvent += subscriber.OnNotificationReceived; 
publisher.NotificationEvent += subscriber2.OnNotificationReceived;

publisher.PublishMessage("Hello, subscribers!"); // Publish a message

Console.ReadKey();