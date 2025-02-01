namespace Domain.Products
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public int Price { get; private set; }

        public Product(string title, int price)
        {
            Guard(title, price);

            Id = Guid.NewGuid();
            this.Title = title;
            this.Price = price;
        }

        public void Edit(string title, int price)
        {
            Guard(title, price);
            Title = title;
            Price = price;
        }

        private void Guard(string title, int price)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price));
        }
    }
}
