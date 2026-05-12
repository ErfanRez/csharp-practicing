namespace Producer.Consumer.OneWayBridge;

public class BridgeSystem
{
    public static void Main()
    {
        int k = Random.Shared.Next(1, 6);
        int carCount = Random.Shared.Next(5, 51);

        Console.WriteLine($"=== ONE-WAY BRIDGE SIMULATION ===\n");
        Console.WriteLine($"Cars: {carCount}, Capacity: {k}\n");

        Bridge bridge = new(k);

        Thread bridgeThread = new(bridge.HandleCarQueue);
        bridgeThread.Start();

        for (int i = 0; i < carCount; i++)
        {
            Car car = new(bridge);
            bridge.EnqueueCar(car);

            Thread.Sleep(Random.Shared.Next(1001));
        }

        Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss.fff}] All {carCount} cars enqueued\n");

        bridgeThread.Join();
    }
}

