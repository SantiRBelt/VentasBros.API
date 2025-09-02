using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasBros.Domain.Entities;

namespace VentasBros.Infrastructure.Data.Tables
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.Property(e => e.IsActive)
                .IsRequired();
                
            builder.Property(e => e.CreatedAt)
                .IsRequired();

            // Relación jerárquica
            builder.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para optimización
            builder.HasIndex(c => c.ParentId)
                .HasDatabaseName("IX_Categories_ParentId");
                
            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Categories_IsActive");
        }
    }
}
