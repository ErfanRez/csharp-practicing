namespace Producer.Consumer.CallCenter;

public class Operator
{
    public int Id { get; }

    private readonly CallCenter _callCenter;

    public Operator(int id, CallCenter callCenter)
    {
        Id = id;
        _callCenter = callCenter;
    }

    public void Work()
    {
        while (true)
        {
            Customer customer = _callCenter.GetNextCustomer();

            Console.WriteLine(
                $"Operator {Id} answering Customer {customer.Id}");

            Thread.Sleep(customer.CallDurationSeconds);

            Console.WriteLine(
                $"Operator {Id} finished Customer {customer.Id}");
        }
    }
}
