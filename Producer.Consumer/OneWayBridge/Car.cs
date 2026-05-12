namespace Producer.Consumer.OneWayBridge;

public class Car
{
    private static int carCount = 0;
    public readonly int Id;
    public readonly Direction Direction;
    private readonly Bridge _bridge;

    public Car(Bridge bridge)
    {
        this.Id = Interlocked.Increment(ref carCount);
        this.Direction = Random.Shared.Next(2) == 0 ? Direction.Left : Direction.Right;
        this._bridge = bridge;
    }

    public void CrossBridge()
    {
        var delay = Random.Shared.Next(1000, 2001);
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Car {Id} from {Direction} STARTED crossing ({delay}ms)");
        Thread.Sleep(delay);
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Car {Id} from {Direction} FINISHED crossing");

        try
        {
            _bridge.BridgeLimit.Release();
        }
        catch (SemaphoreFullException)
        {
            // ignore
        }
    }
}

