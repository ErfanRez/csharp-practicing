using System;
using System.Collections.Generic;
using System.Text.Json;
using DomainLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Mapping
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasIndex(b => b.Id);

            builder.Property(b => b.ProductName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.ImageName)
                .IsRequired()
                .HasMaxLength(110);


            builder.Property(b => b.Description)
                .IsRequired();

            builder.Property(b => b.Tags)
                .HasConversion(
                    data => JsonSerializer.Serialize(data,null),
                    data => JsonSerializer.Deserialize<List<string>>(data,null));

            builder.HasData(new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    ProductName = "Mobile",
                    ImageName = "test.png",
                    Description = "Testtt"
                }
            });
        }
    }
}