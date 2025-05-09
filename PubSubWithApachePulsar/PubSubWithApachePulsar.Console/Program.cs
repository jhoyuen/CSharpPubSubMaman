using DotPulsar;
using DotPulsar.Extensions;
using System.Text;

await using var client = PulsarClient.Builder()
    .ServiceUrl(new Uri("pulsar://localhost:6650"))
    .RetryInterval(TimeSpan.FromSeconds(1))
    .Build();


await using var producer = client.NewProducer()
    .Topic("persistent://public/default/mytopic")
    .Create();

var tmessage = Encoding.UTF8.GetBytes("Hello, Pulsar!");
var tmessageId = await producer.Send(tmessage);

Console.WriteLine($"Message sent with ID: {tmessage}");


await using var consumer = client.NewConsumer()
            .Topic("persistent://public/default/mytopic")
            .SubscriptionName("my-subscription")
            .SubscriptionType(SubscriptionType.Exclusive)
            .Create();

Console.WriteLine("Waiting for messages...");

var cancellationToken = CancellationToken.None;

await foreach (var message in consumer.Messages(cancellationToken))
{
    var content = Encoding.UTF8.GetString(message.Data);
    Console.WriteLine($"Received: {content}");

    // Acknowledge the message
    await consumer.Acknowledge(message, cancellationToken);
}

await using var consumer2 = client.NewConsumer()
            .Topic("persistent://public/default/mytopic")
            .SubscriptionName("my-subscription-2")
            .SubscriptionType(SubscriptionType.Exclusive)
            .Create();

Console.WriteLine("Waiting for messages...");

await foreach (var message in consumer2.Messages(cancellationToken))
{
    var content = Encoding.UTF8.GetString(message.Data);
    Console.WriteLine($"Received: {content}");

    // Acknowledge the message
    await consumer2.Acknowledge(message, cancellationToken);
}

Console.WriteLine("All messages acknowledged.");

