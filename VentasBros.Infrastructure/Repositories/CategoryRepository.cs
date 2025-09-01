using VentasBros.Domain.Entities;
using VentasBros.Domain.Interfaces;
using VentasBros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace VentasBros.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Ejemplo de método específico
        public async Task<IEnumerable<Category>> GetActiveHierarchyAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .Include(c => c.Children)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetByParentIdAsync(int? parentId)
        {
            return await _dbSet
                .Where(c => c.ParentId == parentId)
                .ToListAsync();
        }
    }
}
