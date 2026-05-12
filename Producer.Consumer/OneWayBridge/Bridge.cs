namespace Producer.Consumer.OneWayBridge;

public class Bridge
{
    private readonly int k;
    private readonly Queue<Car> _queue = new();
    public readonly SemaphoreSlim BridgeLimit;
    private Direction _direction = Direction.None;
    private readonly object _lock = new();

    public Bridge(int k)
    {
        this.k = k;
        this.BridgeLimit = new(k, k);
    }

    public void EnqueueCar(Car car)
    {
        lock (_lock)
        {
            _queue.Enqueue(car);
            Console.WriteLine($"Car {car.Id} from {car.Direction} waiting...");
        }
    }

    public void HandleCarQueue()
    {
        while (true)
        {
            if (_queue.TryPeek(out var car))
            {
                lock (_lock)
                {
                    if (BridgeLimit.CurrentCount == k)
                    {
                        _direction = car.Direction;
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Bridge EMPTY. Direction: {_direction}");
                    }

                    // Check if car can enter
                    if (car.Direction == _direction && BridgeLimit.CurrentCount > 0)
                    {
                        BridgeLimit.Wait();
                        _queue.TryDequeue(out _);

                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Car {car.Id} from {car.Direction} ENTERS. " +
                                        $"Permits: {BridgeLimit.CurrentCount}/{k}");

                        Thread crossingThread = new(car.CrossBridge);

                        crossingThread.Start();
                    }
                }
            }
        }
    }
}

