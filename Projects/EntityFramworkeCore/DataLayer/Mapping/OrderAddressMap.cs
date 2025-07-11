using DomainLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Mapping
{
    public class OrderAddressMap : IEntityTypeConfiguration<OrderAddress>
    {
        public void Configure(EntityTypeBuilder<OrderAddress> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Address)
                .IsRequired()
                .HasMaxLength(1000)
                .HasColumnName("Add")
                .HasColumnType("varchar");

            builder.HasOne(b => b.Order)
                .WithOne(b => b.OrderAddress)
                .HasForeignKey<OrderAddress>(b => b.OrderId);
        }
    }
}