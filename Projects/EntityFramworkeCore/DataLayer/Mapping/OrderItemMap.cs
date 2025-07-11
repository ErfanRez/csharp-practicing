using DomainLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Mapping
{
    public class OrderItemMap : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Color)
                .IsRequired()
                .HasMaxLength(50);



            builder.HasOne(b => b.Order)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(b => b.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.UserProduct)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(b => b.UserProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}