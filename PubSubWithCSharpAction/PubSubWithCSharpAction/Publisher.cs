namespace PubSubWithCSharpAction;

public class Publisher
{
    public event Action<string>? NotificationEvent;

    public void PublishMessage(string message)
    {
        NotificationEvent?.Invoke(message);
    }
}
