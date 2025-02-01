using Application.Orders.DTOs;

namespace Application.Orders;

public interface IOrderService
{
    void AddOrder(AddOrderDto dto);
    void FinallyOrder(FinallyOrderDto dto);
    OrderDto GetOrderById(long id);
    List<OrderDto> GetAllOrders();
}
