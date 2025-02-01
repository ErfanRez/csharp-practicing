namespace Domain.Orders
{
    public class Order
    {
        public long Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Count { get; private set; }
        public int Price { get; private set; }
        public int TotalPrice => Count * Price;
        public bool IsFinally { get; private set; }
        public DateTime FinallyDate { get; private set; }
        public Order(Guid productId, int count, int price)
        {
            if (count < 1)
                throw new ArgumentException();

            if (price < 0)
                throw new ArgumentOutOfRangeException();

            ProductId = productId;
            Count = count;
            Price = price;
        }
        public void Finally()
        {
            IsFinally = true;
            FinallyDate = DateTime.Now;
        }
        public void IncreaseProductCount(int count)
        {
            if (count < 1)
                throw new ArgumentException();
            Count = +count;
        }
    }
}
