using VentasBros.Domain.Entities;

namespace VentasBros.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveHierarchyAsync();
        Task<IEnumerable<Category>> GetByParentIdAsync(int? parentId);
    }
}
