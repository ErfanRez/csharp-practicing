using Domain.Orders;
using Domain.Orders.Repository;

namespace Infrastructure.Persistence.Memory.Orders;

public class OrderRepository : IOrderRepository
{
    private Context _context;
    public void Add(Order order)
    {
        _context.Orders.Add(order);
    }

    public List<Order> GetAll()
    {
        return _context.Orders;
    }

    public Order GetById(long id)
    {
        return _context.Orders.FirstOrDefault(p => p.Id == id);
    }

    public void Remove(Order order)
    {
        _context.Orders.Remove(order);
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public void Update(Order order)
    {
        var oldOrder = GetById(order.Id);
        _context.Orders.Remove(oldOrder);
        Add(order);
    }
}
