using VentasBros.Domain.Entities;

namespace VentasBros.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<(IEnumerable<Product> Items, int Total)> GetPagedWithFiltersAsync(
            int page, int pageSize, string? search = null, int? categoryId = null, bool onlyActive = false);
        Task<Product?> GetByIdWithImagesAsync(int id);
    }
}
