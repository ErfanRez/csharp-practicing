using System;
using System.Collections.Generic;
using DomainLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Mapping
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(b => b.Id);
            builder.HasIndex(b => b.Email).IsUnique();

            builder.Property(b => b.Name)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(b => b.Family)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(b => b.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Name)
                .IsRequired(false)
                .HasMaxLength(100);


            builder.Property(b => b.CreationDate)
                .HasDefaultValueSql("GetDate()");

            builder.Ignore(b => b.FullName);

            builder.HasData(new List<User>()
            {
                new User()
                {
                    Id = 1,
                    CreationDate = DateTime.Now,
                    Email = "test@test.com",
                    Family = "Ashrafi",
                    Name = "mohammad"
                }
            });
        }
    }
}