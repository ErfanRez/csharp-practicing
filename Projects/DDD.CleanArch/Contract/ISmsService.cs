namespace Contract;

public interface ISmsService
{
    void SendSms(SmsBody sms);
}

public class SmsBody
{
    public string PhoneNumber { get; set; }
    public string Message { get; set; }

}
