using Common.Domain;
using Common.Domain.Exceptions;
using Common.Domain.Utils;
using Common.Domain.ValueObjects;
using Shop.Domain.ProductAgg.Services;

namespace Shop.Domain.ProductAgg
{
    public class Product : AggregateRoot
    {

        public string Title { get; private set; }
        public string ImageName { get; private set; }
        public string Description { get; private set; }
        public long CategoryId { get; private set; }
        public long SubCategoryId { get; private set; }
        public long SecondarySubCategoryId { get; private set; }
        public string Slug { get; private set; }
        public SeoData SeoData { get; private set; }
        public List<ProductImage> Images { get; private set; }
        public List<ProductSpecification> Specificaions { get; private set; }

        private Product()
        {

        }

        public Product(string title, string imageName, string description, long categoryId,
            long subCategoryId, long secondarySubCategoryId, string slug, SeoData seoData, IProductDomainService productService)
        {
            Guard(title, slug, imageName, description, productService);
            Title = title;
            ImageName = imageName;
            Description = description;
            CategoryId = categoryId;
            SubCategoryId = subCategoryId;
            SecondarySubCategoryId = secondarySubCategoryId;
            Slug = slug.ToSlug();
            SeoData = seoData;
        }

        public void Edit(string title, string imageName, string description, long categoryId,
            long subCategoryId, long secondarySubCategoryId, string slug, SeoData seoData, IProductDomainService productService)
        {
            Guard(title, slug, imageName, description, productService);
            Title = title;
            ImageName = imageName;
            Description = description;
            CategoryId = categoryId;
            SubCategoryId = subCategoryId;
            SecondarySubCategoryId = secondarySubCategoryId;
            Slug = slug;
            SeoData = seoData;
        }

        public void AddImage(ProductImage image)
        {
            image.ProductId = Id;
            Images.Add(image);
        }

        public void RemoveImage(long id)
        {
            var image = Images.FirstOrDefault(x => x.Id == id);
            if (image != null)
                Images.Remove(image);
        }

        public void SetSpecification(List<ProductSpecification> specifications)
        {
            specifications.ForEach(sp => sp.ProductId = Id);
            this.Specificaions = specifications;
        }

        public void Guard(string title, string slug, string imageName, string description, IProductDomainService productService)
        {
            NullOrEmptyDomainDataException.CheckString(title, nameof(title));
            NullOrEmptyDomainDataException.CheckString(slug, nameof(slug));
            NullOrEmptyDomainDataException.CheckString(imageName, nameof(imageName));
            NullOrEmptyDomainDataException.CheckString(description, nameof(description));

            if (slug != Slug)
                if (productService.SlugExists(slug.ToSlug()))
                    throw new SlugIsDuplicateException();
        }


    }
}
