using Domain.Orders;
using Domain.Products;

namespace Infrastructure.Persistence.Memory;

public class Context
{
    public List<Product> Products { get; set; }
    public List<Order> Orders { get; set; }
}
