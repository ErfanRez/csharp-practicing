namespace Domain.Products.Repsitory;

public interface IProductRepository
{
    List<Product> GetAll();
    Product GetById(Guid id);
    void Add(Product product);
    void Update(Product product);
    void Remove(Product product);
    void SaveChanges();
}
