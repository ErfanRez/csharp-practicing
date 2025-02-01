using Application.Products.DTOs;
using Domain.Products;
using Domain.Products.Repsitory;

namespace Application.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public void AddProduct(AddProductDto dto)
    {
        _repository.Add(new Product(dto.Title, dto.Price));
        _repository.SaveChanges();
    }

    public void EditProduct(EditProductDto dto)
    {
        var product = _repository.GetById(dto.Id);
        product.Edit(dto.Title, dto.Price);

        _repository.Update(product);
        _repository.SaveChanges();
    }

    public ProductDto GetProductById(Guid productId)
    {
        var product = _repository.GetById(productId);
        return new ProductDto()
        {
            Price = product.Price,
            Id = productId,
            Title = product.Title
        };
    }

    public List<ProductDto> GetProducts()
    {
        return _repository.GetAll().Select(product => new ProductDto()
        {
            Price = product.Price,
            Id = product.Id,
            Title = product.Title
        }).ToList();

    }
}