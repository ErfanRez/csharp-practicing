namespace Application.Orders.DTOs;

public class OrderDto
{
    public long Id { get; set; }
    public Guid ProductId { get; set; }
    public int Count { get; set; }
    public int Price { get; set; }

}
