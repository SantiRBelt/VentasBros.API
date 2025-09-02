using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasBros.Domain.Entities;

namespace VentasBros.Infrastructure.Data.Tables
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();
                
            builder.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
                
            builder.Property(e => e.CategoryId)
                .IsRequired();
                
            builder.Property(e => e.IsActive)
                .IsRequired();
                
            builder.Property(e => e.CreatedAt)
                .IsRequired();

            // Relación con Category
            builder.HasOne(e => e.Category)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para optimización
            builder.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");
                
            builder.HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Products_IsActive");
        }
    }
}
