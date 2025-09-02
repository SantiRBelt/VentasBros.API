using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasBros.Domain.Entities;

namespace VentasBros.Infrastructure.Data.Tables
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Username)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.Property(e => e.Email)
                .HasMaxLength(256)
                .IsRequired();
                
            builder.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .IsRequired();
                
            builder.Property(e => e.Role)
                .HasMaxLength(50)
                .IsRequired();
                
            builder.Property(e => e.IsActive)
                .IsRequired();
                
            builder.Property(e => e.CreatedAt)
                .IsRequired();

            // Índices únicos
            builder.HasIndex(e => e.Username)
                .IsUnique();
                
            builder.HasIndex(e => e.Email)
                .IsUnique();
        }
    }
}
