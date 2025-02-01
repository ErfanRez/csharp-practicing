namespace Application.Orders.DTOs;

public class AddOrderDto
{
    public Guid ProductId { get; set; }
    public int Count { get; set; }
    public int Price { get; set; }
}
