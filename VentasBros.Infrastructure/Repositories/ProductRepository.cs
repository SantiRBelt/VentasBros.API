using Microsoft.EntityFrameworkCore;
using VentasBros.Domain.Entities;
using VentasBros.Domain.Interfaces;
using VentasBros.Infrastructure.Data;

namespace VentasBros.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetByIdWithImagesAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(IEnumerable<Product> Items, int Total)> GetPagedWithFiltersAsync(
            int page, int pageSize, string? search = null, int? categoryId = null, bool onlyActive = false)
        {
            var query = _dbSet.Include(p => p.Category).Include(p => p.Images).AsQueryable();

            if (onlyActive)
                query = query.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
