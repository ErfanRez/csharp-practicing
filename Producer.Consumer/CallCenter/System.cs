namespace Producer.Consumer.CallCenter;

public class System
{
    public static void Build()
    {
        var operatorCount = Random.Shared.Next(2, 11);
        var customerCount = Random.Shared.Next(10, 41);

        Console.WriteLine("========================================");
        Console.WriteLine("CALL CENTER SIMULATION STARTED");
        Console.WriteLine("========================================");
        Console.WriteLine($"Operators : {operatorCount}");
        Console.WriteLine($"Customers : {customerCount}");
        Console.WriteLine();

        var callCenter = new CallCenter();

        for (int i = 0; i < operatorCount; i++)
        {
            var op = new Operator(i + 1, callCenter);

            Console.WriteLine(
               $"[{DateTime.Now:HH:mm:ss.fff}] Operator {op.Id} is online");

            var thread = new Thread(op.Work)
            {
                IsBackground = true,
            };

            thread.Start();
        }

        Console.WriteLine();

        for (int i = 0; i < customerCount; i++)
        {
            var callDuration = Random.Shared.Next(1000, 4001);
            var customer = new Customer(callDuration);

            callCenter.EnqueueCustomer(customer);

            var delay = Random.Shared.Next(1001);
            Thread.Sleep(delay);
        }
    }
}
