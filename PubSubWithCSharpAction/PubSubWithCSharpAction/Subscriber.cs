namespace PubSubWithCSharpAction;

public class Subscriber
{
    public void OnNotificationReceived(string message)
    {
        Console.WriteLine($"Received and processing message: {message}");
    }
}
