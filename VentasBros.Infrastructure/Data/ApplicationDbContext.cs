using Microsoft.EntityFrameworkCore;
using VentasBros.Domain.Entities;

namespace VentasBros.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<JwtToken> JwtTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.CategoryId).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasOne(e => e.Category)
                    .WithMany(e => e.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Image configuration
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Url).HasMaxLength(500).IsRequired();
                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.IsMain).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasOne(e => e.Product)
                    .WithMany(e => e.Images)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // JwtToken configuration
            modelBuilder.Entity<JwtToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).HasMaxLength(500).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.IsRevoked).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany(e => e.JwtTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Índices adicionales para optimización
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Products_IsActive");

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.ParentId)
                .HasDatabaseName("IX_Categories_ParentId");

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Categories_IsActive");

            modelBuilder.Entity<Image>()
                .HasIndex(i => i.ProductId)
                .HasDatabaseName("IX_Images_ProductId");

            modelBuilder.Entity<JwtToken>()
                .HasIndex(j => j.UserId)
                .HasDatabaseName("IX_JwtTokens_UserId");

            modelBuilder.Entity<JwtToken>()
                .HasIndex(j => j.Token)
                .HasDatabaseName("IX_JwtTokens_Token");
        }
    }
}
