using System.Collections.Concurrent;

namespace Producer.Consumer.CallCenter;

public class CallCenter
{
    private readonly ConcurrentQueue<Customer> _queue = new();
    private readonly SemaphoreSlim _waitingCustomers = new(0);

    public void EnqueueCustomer(Customer customer)
    {
        _queue.Enqueue(customer);

        Console.WriteLine(
            $"Customer {customer.Id} entered queue ({customer.CallDurationSeconds}s)");

        _waitingCustomers.Release();
    }

    public Customer GetNextCustomer()
    {
        _waitingCustomers.Wait();

        _queue.TryDequeue(out var customer);

        return customer!;
    }
}