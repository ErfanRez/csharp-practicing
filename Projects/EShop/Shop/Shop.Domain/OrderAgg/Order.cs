using Common.Domain;
using Common.Domain.Exceptions;
using Shop.Domain.OrderAgg.ValueObjects;

namespace Shop.Domain.OrderAgg;

public class Order : AggregateRoot
{
    private Order()
    {

    }

    public Order(long userId)
    {
        UserId = userId;
        Status = OrderStatus.Pending;
        Items = new List<OrderItem>();

    }

    public long UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public OrderDiscount? Discount { get; private set; }
    public OrderAddress? Address { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public int TotalPrice => Items.Sum(s => s.TotalPrice);

    public DateTime? LastUpdate { get; set; }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
    }

    public void RemoveItem(long itemId)
    {
        var currItem = Items.FirstOrDefault(i => i.Id == itemId);
        if (currItem != null)
            Items.Remove(currItem);
    }

    public void ChangeCount(long itemId, int newCount)
    {
        var currItem = Items.FirstOrDefault(i => i.Id == itemId);
        if (currItem == null)
            throw new NullOrEmptyDomainDataException();

        currItem.ChangeCount(newCount);
    }

    public void ChangeStatus(OrderStatus status)
    {
        Status = status;
        LastUpdate = DateTime.Now;
    }

    public void Checkout(OrderAddress orderAddress)
    {
        Address = orderAddress;
    }

}

