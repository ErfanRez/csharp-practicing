namespace Shop.Domain.ProductAgg.Services
{
    public interface IProductDomainService
    {
        bool SlugExists(string slug);
    }
}
