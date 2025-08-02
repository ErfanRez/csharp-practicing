using System.Collections.Generic;
using DomainLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Mapping
{
    public class UserProductMap : IEntityTypeConfiguration<UserProduct>
    {
        public void Configure(EntityTypeBuilder<UserProduct> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Color)
                .IsRequired()
                .HasMaxLength(50);


            builder.HasOne(b => b.User)
                .WithMany(b => b.Products)
                .HasForeignKey(b => b.SellerId);

            builder.HasOne(b => b.Product)
                .WithMany(b => b.UserProducts)
                .HasForeignKey(b => b.ProductId);


            builder.HasData(new List<UserProduct>()
            {
                new UserProduct()
                {
                    Id = 1,
                    ProductId = 1,
                    SellerId = 1,
                    Color = "Red",
                    Price = 1000000
                }
            });
        }
    }
}