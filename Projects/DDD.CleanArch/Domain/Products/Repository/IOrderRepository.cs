namespace Domain.Products.Repsitory;

public interface IProductRepository
{
    List<Product> GetAll();
    Product GetById(Guid id);
    void Add(Product Product);
    void Update(Product Product);
    void SaveChanges();
}
