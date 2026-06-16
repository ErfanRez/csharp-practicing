namespace Producer.Consumer.CallCenter;

public class Customer
{
    private static int _nextId;

    public int Id { get; }
    public int CallDurationSeconds { get; }

    public Customer(int callDurationSeconds)
    {
        Id = Interlocked.Increment(ref _nextId);
        CallDurationSeconds = callDurationSeconds;
    }
}
