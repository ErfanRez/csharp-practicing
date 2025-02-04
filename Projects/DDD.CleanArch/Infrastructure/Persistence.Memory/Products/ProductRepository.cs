using Domain.Products;
using Domain.Products.Repsitory;

namespace Infrastructure.Persistence.Memory.Products;

public class ProductRepository : IProductRepository
{
    private Context _context;

    public ProductRepository(Context context)
    {
        _context = context;
    }
    public void Add(Product product)
    {
        _context.Products.Add(product);
    }

    public List<Product> GetAll()
    {
        return _context.Products;
    }

    public Product GetById(Guid id)
    {
        return _context.Products.FirstOrDefault(p => p.Id == id);
    }

    public void Remove(Product product)
    {
        _context.Products.Remove(product);
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public void Update(Product product)
    {
        var oldProduct = GetById(product.Id);
        _context.Products.Remove(oldProduct);
        Add(product);
    }
}
