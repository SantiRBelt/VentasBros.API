using System.Collections.Generic;
using System.Threading.Tasks;
using VentasBros.Domain.Entities;

namespace VentasBros.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveAsync();
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetByParentIdAsync(int? parentId);
        Task<IEnumerable<Category>> GetActiveHierarchyAsync();
    }
}
