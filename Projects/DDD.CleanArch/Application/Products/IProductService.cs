using Application.Products.DTOs;

namespace Application.Products;

public interface IProductService
{
    void AddProduct(AddProductDto dto);
    void EditProduct(EditProductDto dto);
    ProductDto GetProductById(Guid productId);
    List<ProductDto> GetProducts();
}