using System;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using DomainLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataLayer.Mapping
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "dbo");
            builder.HasKey(b => b.Id);


            //builder.Property(b => b.Status)
            //    .HasConversion(v => v.ToString(),
            //        v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v));

            //builder.Property(b => b.Status)
            //    .HasConversion<string>();

            //builder.Property(b => b.Status)
            //    .HasConversion(new EnumToStringConverter<OrderStatus>());

            //var conversion = new ValueConverter<OrderStatus, string>(
            //    data => data.ToString(),
            //    data => (OrderStatus) Enum.Parse(typeof(OrderStatus), data));

            //builder.Property(b => b.Status)
            //    .HasConversion(conversion);
        }
    }
}