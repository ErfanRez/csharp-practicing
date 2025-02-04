using Application.Orders.DTOs;
using Contract;
using Domain.Orders;
using Domain.Orders.Repository;

namespace Application.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly ISmsService _smsServce;

    public OrderService(IOrderRepository repository, ISmsService smsService)
    {
        _repository = repository;
        _smsServce = smsService;
    }

    public void AddOrder(AddOrderDto dto)
    {
        var order = new Order(dto.ProductId, dto.Count, dto.Price);
        _repository.Add(order);
        _repository.SaveChanges();
    }

    public void FinallyOrder(FinallyOrderDto dto)
    {
        var order = _repository.GetById(dto.OrderId);
        order.Finally();
        _repository.Update(order);
        _repository.SaveChanges();
        _smsServce.SendSms(new SmsBody()
        {
            Message = "Done!",
            PhoneNumber = "09031775736"
        });
    }

    public List<OrderDto> GetAllOrders()
    {
        return _repository.GetAll().Select(order => new OrderDto()
        {
            Id = order.Id,
            Count = order.Count,
            Price = order.Price,
            ProductId = order.ProductId,
        }).ToList();

    }

    public OrderDto GetOrderById(long id)
    {
        var order = _repository.GetById(id);
        return new OrderDto()
        {
            Id = id,
            Count = order.Count,
            Price = order.Price,
            ProductId = order.ProductId,
        };
    }
}