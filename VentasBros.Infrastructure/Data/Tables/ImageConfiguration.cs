using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasBros.Domain.Entities;

namespace VentasBros.Infrastructure.Data.Tables
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Url)
                .HasMaxLength(500)
                .IsRequired();
                
            builder.Property(e => e.ProductId)
                .IsRequired();
                
            builder.Property(e => e.IsMain)
                .IsRequired();
                
            builder.Property(e => e.CreatedAt)
                .IsRequired();

            // Relación con Product
            builder.HasOne(e => e.Product)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para optimización
            builder.HasIndex(i => i.ProductId)
                .HasDatabaseName("IX_Images_ProductId");
        }
    }
}
