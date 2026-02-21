namespace SMS.Consumer.Data;

internal class SMSData
{
    public static readonly Dictionary<long, List<bool>> SMSDic = [];

    public static readonly HashSet<string> DeDupeSet = new();

}
