namespace Structural;

class AdapterProgram
{
    public static void Main()
    {
        ISmsProvider smsProvider = new SmsProvider();
        IEnumerable<INotificationService> notificationServices = [new CustomNotificationService(), new SmsNotificationService(smsProvider)];
        Dispatcher dispatcher = new(notificationServices);
        dispatcher.SendNotification();
    }

}

class Dispatcher
{
    private readonly IEnumerable<INotificationService> _notificationService;
    public Dispatcher(IEnumerable<INotificationService> notificationService)
    {
        _notificationService = notificationService;
    }

    public void SendNotification()
    {
        foreach (INotificationService notificationService in _notificationService)
        {
            notificationService.SendAsync("recipient", "subject", "message");
        }
    }
}

interface INotificationService
{
    void SendAsync(
        string recipient,
        string subject,
        string message);
}

class CustomNotificationService : INotificationService
{
    public void SendAsync(string recipient, string subject, string message)
    {
        // notification processing logic
    }
}

interface ISmsProvider
{
    void SendText(string phoneNumber, string text);
}

class SmsProvider : ISmsProvider
{
    public void SendText(
        string phoneNumber,
        string text)
    {

    }
}

// Adapter class
class SmsNotificationService : INotificationService
{
    // Adaptee
    private readonly ISmsProvider _smsProvider;

    public SmsNotificationService(ISmsProvider smsProvider)
    {
        _smsProvider = smsProvider;
    }

    public void SendAsync(string recipient, string subject, string message)
    {
        _smsProvider.SendText(phoneNumber: recipient, text: message);
    }
}