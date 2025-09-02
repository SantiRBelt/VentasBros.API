using VentasBros.Domain.Entities;
using VentasBros.Domain.Interfaces;
using VentasBros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace VentasBros.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetActiveAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .Include(c => c.Parent)
                .Include(c => c.Children.Where(child => child.IsActive))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(c => c.Parent)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Category>> GetByParentIdAsync(int? parentId)
        {
            return await _dbSet
                .Where(c => c.ParentId == parentId)
                .Include(c => c.Parent)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetActiveHierarchyAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .Include(c => c.Children.Where(child => child.IsActive))
                .Where(c => c.ParentId == null) // Solo categorías raíz
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public override async Task<Category?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
