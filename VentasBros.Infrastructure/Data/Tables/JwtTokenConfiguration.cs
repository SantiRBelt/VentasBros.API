using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasBros.Domain.Entities;

namespace VentasBros.Infrastructure.Data.Tables
{
    public class JwtTokenConfiguration : IEntityTypeConfiguration<JwtToken>
    {
        public void Configure(EntityTypeBuilder<JwtToken> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Token)
                .HasMaxLength(500)
                .IsRequired();
                
            builder.Property(e => e.UserId)
                .IsRequired();
                
            builder.Property(e => e.ExpiresAt)
                .IsRequired();
                
            builder.Property(e => e.IsRevoked)
                .IsRequired();
                
            builder.Property(e => e.CreatedAt)
                .IsRequired();

            // Relación con User
            builder.HasOne(e => e.User)
                .WithMany(e => e.JwtTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para optimización
            builder.HasIndex(j => j.UserId)
                .HasDatabaseName("IX_JwtTokens_UserId");
                
            builder.HasIndex(j => j.Token)
                .HasDatabaseName("IX_JwtTokens_Token");
        }
    }
}
