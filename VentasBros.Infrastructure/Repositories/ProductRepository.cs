using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            int page,
            int pageSize,
            string? search = null,
            int? categoryId = null,
            bool onlyActive = false,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sort = "createdAt",
            string direction = "desc")
        {
            var query = _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsQueryable();

            // Filtro por estado activo
            if (onlyActive)
                query = query.Where(p => p.IsActive);

            // Filtro por búsqueda de texto
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    (p.Description != null && p.Description.Contains(search)) ||
                    p.Category.Name.Contains(search));
            }

            // Filtro por categoría
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            // Filtro por precio mínimo
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            // Filtro por precio máximo
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Aplicar ordenamiento
            query = ApplySorting(query, sort, direction);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string sort, string direction)
        {
            var isDescending = direction.ToLower() == "desc";

            return sort.ToLower() switch
            {
                "name" => isDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),

                "price" => isDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),

                "category" => isDescending
                    ? query.OrderByDescending(p => p.Category.Name)
                    : query.OrderBy(p => p.Category.Name),

                "createdat" or "created" => isDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),

                _ => query.OrderByDescending(p => p.CreatedAt) // Default: más recientes primero
            };
        }
    }
}
