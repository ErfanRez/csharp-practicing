namespace Domain.Orders.Repository
{
    public interface IOrderRepository
    {
        List<Order> GetAll();
        Order GetById(long id);
        void Add(Order order);
        void Update(Order order);

        void SaveChanges();
    }
}
